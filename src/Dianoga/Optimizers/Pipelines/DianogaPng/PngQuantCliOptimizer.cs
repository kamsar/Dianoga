namespace Dianoga.Optimizers.Pipelines.DianogaPng
{
	// uses pngquant: https://pngquant.org/
	public class PngQuantCliOptimizer : PngOptimizer
	{
		protected override bool OptimizerUsesSeparateOutputFile => false;

		protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
		{
			return $"--force --ext .png \"{tempFilePath}\"";
		}
	}
}
