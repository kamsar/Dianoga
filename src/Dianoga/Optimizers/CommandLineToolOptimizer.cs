using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;

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
			get { return _pathToExe; }
			set
			{
				if (value.StartsWith("~") || value.StartsWith("/")) _pathToExe = HostingEnvironment.MapPath(value);
				else _pathToExe = value;
			}
		}

		/// <summary>
		/// Additional arguments to pass to the executable beyond the required ones (e.g. optimization options)
		/// </summary>
		public virtual string AdditionalToolArguments
		{
			get { return _additionalToolArguments; }
			set
			{
				if (!string.IsNullOrEmpty(value) && value.StartsWith("-"))
					_additionalToolArguments = value;
			}
		}

		/// <summary>
		/// Determines if the optimizer optimizes the temporary file in-place (overwriting it) or writes to a separate output path
		/// The default is to use a separate output file; override and set to false if that is not an option.
		/// </summary>
		protected virtual bool OptimizerUsesSeparateOutputFile => true;

		/// <summary>
		/// Shell execute uses PATH and can run non-exe files (e.g. cmd) but it cannot capture output
		/// </summary>
		protected bool UseShellExecute { get; set; }

		protected override void ProcessOptimizer(OptimizerArgs args)
		{
			var tempFilePath = GetTempFilePath();
			var tempOutputPath = OptimizerUsesSeparateOutputFile ? GetTempFilePath() : null;

			var arguments = CreateToolArguments(tempFilePath, tempOutputPath);

			if (!string.IsNullOrEmpty(AdditionalToolArguments))
				arguments = AdditionalToolArguments.TrimEnd() + " " + arguments;

			try
			{
				using (var fileStream = File.OpenWrite(tempFilePath))
				{
					args.Stream.CopyTo(fileStream);
					args.Stream.Dispose();
				}

				if (UseShellExecute) ExecuteShell(arguments);
				else ExecuteProcess(arguments);

				// read the file to memory so we can nuke the temp file
				var outputPath = OptimizerUsesSeparateOutputFile ? tempOutputPath : tempFilePath;
				using (var fileStream = File.OpenRead(outputPath))
				{
					args.Stream = new MemoryStream();
					fileStream.CopyTo(args.Stream);
				}
			}
			finally
			{
				if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
				if (tempOutputPath != null && File.Exists(tempOutputPath)) File.Delete(tempOutputPath);
			}

			args.IsOptimized = true;
		}

		protected virtual void ExecuteProcess(string arguments)
		{
			var processOutput = new ConcurrentBag<string>();

			var processInfo = new ProcessStartInfo
			{
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				FileName = ExePath,
				Arguments = arguments
			};

			var toolProcess = new Process();
			toolProcess.StartInfo = processInfo;
			toolProcess.OutputDataReceived += (sender, eventArgs) => processOutput.Add(eventArgs.Data);
			toolProcess.ErrorDataReceived += (sender, eventArgs) => processOutput.Add(eventArgs.Data);

			toolProcess.Start();
			toolProcess.BeginOutputReadLine();
			toolProcess.BeginErrorReadLine();

			if (!toolProcess.WaitForExit(ToolTimeout))
			{
				try
				{
					toolProcess.Kill();
				}
				catch
				{
					// do nothing if kill errors, we want the exception below
				}

				throw new InvalidOperationException($"{ExePath} took longer than {ToolTimeout}ms to run, which is a failure. Output: {string.Join(Environment.NewLine, processOutput)}");
			}

			if (toolProcess.ExitCode != 0)
			{
				throw new InvalidOperationException($"{ExePath} exited with unexpected exit code {toolProcess.ExitCode}. Output: {string.Join(Environment.NewLine, processOutput)}");
			}
		}

		protected virtual void ExecuteShell(string arguments)
		{
			var processInfo = new ProcessStartInfo
			{
				UseShellExecute = true,
				FileName = ExePath,
				Arguments = arguments
			};

			var toolProcess = System.Diagnostics.Process.Start(processInfo);

			// ReSharper disable once PossibleNullReferenceException
			if (!toolProcess.WaitForExit(ToolTimeout))
			{
				try
				{
					toolProcess.Kill();
				}
				catch
				{
					// do nothing if kill errors, we want the exception below
				}

				throw new InvalidOperationException($"{ExePath} took longer than {ToolTimeout}ms to run, which is a failure. Output not available using shell execute.");
			}

			if (toolProcess.ExitCode != 0)
			{
				throw new InvalidOperationException($"{ExePath} exited with unexpected exit code {toolProcess.ExitCode}. Output not available using shell execute.");
			}
		}

		protected abstract string CreateToolArguments(string tempFilePath, string tempOutputPath);

		protected virtual string GetTempFilePath()
		{
			return Path.GetTempFileName();
		}

		protected virtual int ToolTimeout => 60000; // in msec
	}
}
