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
			// must have a PNG extension and the default gives us .tmp
			return Path.ChangeExtension(base.GetTempFilePath(), ".png");
		}
	}
}
