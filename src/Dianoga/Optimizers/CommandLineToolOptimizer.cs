using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using Sitecore.Configuration;

namespace Dianoga.Optimizers
{
	/// <summary>
	/// Optimizes images using a command-line tool that acts on temp files.
	/// </summary>
	public abstract class CommandLineToolOptimizer : OptimizerProcessor
	{
		private string _pathToExe;

		private string _additionalToolArguments;

		public virtual string ExePath
		{
			get => _pathToExe;
			set
			{
				if (value.StartsWith("~") || value.StartsWith("/")) _pathToExe = HostingEnvironment.MapPath(value) ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value.TrimStart('/', '\\'));
				else _pathToExe = value;
			}
		}

		/// <summary>
		/// Additional arguments to pass to the executable beyond the required ones (e.g. optimization options)
		/// </summary>
		public virtual string AdditionalToolArguments
		{
			get => _additionalToolArguments;
			set
			{
				if (!string.IsNullOrEmpty(value) && value.StartsWith("-"))
					_additionalToolArguments = value;
			}
		}

		/// <summary>
		/// If true, additional arguments are prepended to the arguments from CreateToolArguments()
		/// If false, additional arguments are added at the end
		/// </summary>
		protected virtual bool PrependAdditionalArguments => true;

		/// <summary>
		/// Determines if the optimizer optimizes the temporary file in-place (overwriting it) or writes to a separate output path
		/// The default is to use a separate output file; override and set to false if that is not an option.
		/// </summary>
		protected virtual bool OptimizerUsesSeparateOutputFile => true;

		/// <summary>
		/// Process execute timeout in ms
		/// </summary>
		protected virtual int ToolTimeout => 60000;

		protected override void ProcessOptimizer(OptimizerArgs args)
		{
			var tempFilePath = GetTempFilePath();
			var tempOutputPath = OptimizerUsesSeparateOutputFile ? GetTempFilePath() : null;

			var arguments = GenerateArguments(args, tempFilePath, tempOutputPath);

			try
			{
				// First copy the stream to the temp source file
				using (var fileStream = File.OpenWrite(tempFilePath))
				{
					args.Stream.CopyTo(fileStream);
				}

				// Execute the command line tool
				ExecuteProcess(arguments);

				// Read the output file to memory and nuke the temp file
				var outputPath = OptimizerUsesSeparateOutputFile ? tempOutputPath : tempFilePath;
				using (var fileStream = File.OpenRead(outputPath))
				{
					// If we got here everything has gone well so dispose the original stream and 
					// write our optimised stream to it
					args.Stream.Dispose();
					args.Stream = new MemoryStream();
					fileStream.CopyTo(args.Stream);
					args.IsOptimized = true;
				}
			}
			finally
			{
				if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
				if (tempOutputPath != null && File.Exists(tempOutputPath)) File.Delete(tempOutputPath);
			}
		}

		/// <summary>
		/// Generate arguments by merging default tool arguments with any possible additional arguments
		/// </summary>
		protected virtual string GenerateArguments(OptimizerArgs args, string tempFilePath, string tempOutputPath)
		{
			var arguments = CreateToolArguments(tempFilePath, tempOutputPath);

			if (!string.IsNullOrEmpty(AdditionalToolArguments))
			{
				arguments = PrependAdditionalArguments ?
					$"{AdditionalToolArguments.TrimEnd()} {arguments}" :
					$"{arguments.TrimEnd()} {AdditionalToolArguments.TrimEnd()}";
			}

			var mediaSpecificArguments = GetMediaSpecificArguments(args)?.TrimEnd();
			if (!string.IsNullOrEmpty(mediaSpecificArguments))
			{
				arguments = PrependAdditionalArguments ?
					$"{mediaSpecificArguments} {arguments.TrimEnd()}" :
					$"{arguments.TrimEnd()} {mediaSpecificArguments}";
			}

			return arguments;
		}

		/// <summary>
		/// Generate arguments based on the provided media & media options, such as resize parameters
		/// </summary>
		protected virtual string GetMediaSpecificArguments(OptimizerArgs args)
		{
			return null;
		}

		protected virtual void ExecuteProcess(string arguments)
		{

#if DEBUG
			DianogaLog.Info($"\"{ExePath} {arguments}\"");
#endif

			using (var process = new Process())
			{
				process.StartInfo.FileName = ExePath;
				process.StartInfo.Arguments = arguments;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.CreateNoWindow = true;

				var processOutput = new ConcurrentBag<string>();
				process.OutputDataReceived += (sender, eventArgs) => processOutput.Add(eventArgs.Data);
				process.ErrorDataReceived += (sender, eventArgs) => processOutput.Add(eventArgs.Data);

				try
				{
					process.Start();
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException($"\"{ExePath} {arguments}\" could not be started because an error occurred. See the inner exception for details.", ex);
				}

				process.BeginOutputReadLine();
				process.BeginErrorReadLine();


				if (!process.WaitForExit(ToolTimeout))
				{
					try
					{
						process.Kill();
					}
					catch
					{
						// do nothing if kill errors, we want the exception below
					}

					throw new InvalidOperationException($"\"{ExePath} {arguments}\" took longer than {ToolTimeout}ms to run, which is a failure. Output: {string.Join(Environment.NewLine, processOutput)}");
				}

				if (process.ExitCode != 0)
				{
					throw new InvalidOperationException($"\"{ExePath} {arguments}\" exited with unexpected exit code {process.ExitCode}. Output: {string.Join(Environment.NewLine, processOutput)}");
				}
			}
		}

		protected abstract string CreateToolArguments(string tempFilePath, string tempOutputPath);

		protected virtual string GetTempFilePath()
		{
			try
			{
				var tempFilePath = Settings.GetSetting("Dianoga.TempFilePath");
				if (!string.IsNullOrEmpty(tempFilePath))
				{
					return Path.Combine(tempFilePath, Path.GetRandomFileName());
				}
				return Path.GetTempFileName();
			}
			catch (IOException ioe)
			{
				throw new InvalidOperationException($"Error occurred while creating temp file to optimize. This can happen if IIS does not have write access to {Path.GetTempPath()}, or if the temp folder has 65535 files in it and is full.", ioe);
			}
		}
	}
}
