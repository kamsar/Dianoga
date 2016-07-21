using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
		/// Shell execute uses PATH and can run non-exe files (e.g. cmd) but it cannot capture output
		/// </summary>
		protected bool UseShellExecute { get; set; }

		protected override void ProcessOptimizer(OptimizerArgs args)
		{
			var tempFilePath = GetTempFilePath();
			var tempOutputPath = GetTempFilePath();

			var arguments = CreateToolArguments(tempFilePath, tempOutputPath);

			try
			{
				using (var fileStream = File.OpenWrite(tempFilePath))
				{
					args.Stream.CopyTo(fileStream);
					args.Stream.Dispose();
				}

				if(UseShellExecute) ExecuteShell(arguments);
				else ExecuteProcess(arguments);

				// read the file to memory so we can nuke the temp file
				using (var fileStream = File.OpenRead(tempOutputPath))
				{
					args.Stream = new MemoryStream();
					fileStream.CopyTo(args.Stream);
				}
			}
			finally
			{
				if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
				if (File.Exists(tempOutputPath)) File.Delete(tempOutputPath);
			}

			args.IsOptimized = true;
		}

		protected virtual void ExecuteProcess(string arguments)
		{
		    var processOutput = new ConcurrentBag<string>();

			var processInfo = new ProcessStartInfo();
			processInfo.UseShellExecute = false;
			processInfo.RedirectStandardOutput = true;
			processInfo.RedirectStandardError = true;
			processInfo.FileName = ExePath;
			processInfo.Arguments = arguments;

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
			var processInfo = new ProcessStartInfo();
			processInfo.UseShellExecute = true;
			processInfo.FileName = ExePath;
			processInfo.Arguments = arguments;

			var toolProcess = System.Diagnostics.Process.Start(processInfo);

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
