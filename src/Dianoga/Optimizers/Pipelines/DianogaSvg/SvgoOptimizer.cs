using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace Dianoga.Optimizers.Pipelines.DianogaSvg
{
	/// <summary>
	/// Uses https://github.com/twardoch/svgop
	/// </summary>
	public class SvgoOptimizer : CommandLineToolOptimizer
	{

		protected override void ProcessOptimizer(OptimizerArgs args)
		{
			ExecuteProcess(args);
		}

		protected void ExecuteProcess(OptimizerArgs args)
		{
			using (Process toolProcess = new Process())
			{
				toolProcess.StartInfo.FileName = ExePath;
				toolProcess.StartInfo.Arguments = AdditionalToolArguments;
				toolProcess.StartInfo.UseShellExecute = false;
				toolProcess.StartInfo.RedirectStandardInput = true;
				toolProcess.StartInfo.RedirectStandardOutput = true;
				toolProcess.StartInfo.RedirectStandardError = true;

				var processOutput = new ConcurrentBag<string>();
				toolProcess.ErrorDataReceived += (sender, eventArgs) => processOutput.Add(eventArgs.Data);

#if DEBUG
				DianogaLog.Info($"\"{ExePath} {AdditionalToolArguments}\"");
#endif

				try
				{
					toolProcess.Start();
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException($"\"{ExePath}\" could not be started because an error occurred. See the inner exception for details.", ex);
				}

				// Copy the svg to the stdin stream then close it to say we're done
				StreamWriter myStreamWriter = toolProcess.StandardInput;
				args.Stream.CopyTo(myStreamWriter.BaseStream);
				myStreamWriter.Close();

				toolProcess.BeginErrorReadLine();

				// Read the optimized svg from the stdout stream
				args.Stream.Dispose();
				args.Stream = new MemoryStream();
				toolProcess.StandardOutput.BaseStream.CopyTo(args.Stream);
				args.IsOptimized = true;

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

					throw new InvalidOperationException($"\"{ExePath}\" took longer than {ToolTimeout}ms to run, which is a failure. Output: {string.Join(Environment.NewLine, processOutput)}");
				}

				if (toolProcess.ExitCode != 0)
				{
					throw new InvalidOperationException($"\"{ExePath}\" exited with unexpected exit code {toolProcess.ExitCode}. Output: {string.Join(Environment.NewLine, processOutput)}");
				}
			}
		}

		protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
		{
			return string.Empty;
		}
	}
}
