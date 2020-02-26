using System.IO;

namespace Dianoga.Optimizers.Pipelines.DianogaPng
{
	// uses pngquant: https://pngquant.org/
	public class PngQuantCliOptimizer : CommandLineToolOptimizer
	{
		protected override bool OptimizerUsesSeparateOutputFile => false;

		protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
		{
			return $"--force --ext .png \"{tempFilePath}\"";
		}

		protected override string GetTempFilePath()
		{
			// must have a PNG extension and the default gives us .tmp
			return Path.ChangeExtension(base.GetTempFilePath(), ".png");
		}
	}
}
