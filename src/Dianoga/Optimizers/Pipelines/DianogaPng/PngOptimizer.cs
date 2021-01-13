using System;
using System.IO;

namespace Dianoga.Optimizers.Pipelines.DianogaPng
{
	// uses PngOptimizer to crunch PNGs - this is the fastest optimizer by far, and quite effective: http://psydk.org/pngoptimizer
	public class PngOptimizer : CommandLineToolOptimizer
	{
		protected override bool OptimizerUsesSeparateOutputFile => false;

		protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
		{
			return $"-file \"{tempFilePath}\"";
		}

		protected override string GetTempFilePath()
		{
			try
			{
				// must have a PNG extension and GetTempFileName gives us .tmp
				var tmpFileName = base.GetTempFilePath();

				// GetTempFileName actually creates a zero length file; we want to delete that
				// to avoid creating uncontrolled temp files (#36)
				if (File.Exists(tmpFileName)) File.Delete(tmpFileName);

				return Path.ChangeExtension(tmpFileName, ".png");
			}
			catch (IOException ioe)
			{
				throw new InvalidOperationException($"Error occurred while creating temp file to optimize. This can happen if IIS does not have write access to {Path.GetTempPath()}, or if the temp folder has 65535 files in it and is full.", ioe);
			}
		}
	}
}
