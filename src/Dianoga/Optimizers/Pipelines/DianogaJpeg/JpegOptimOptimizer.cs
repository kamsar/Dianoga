namespace Dianoga.Optimizers.Pipelines.DianogaJpeg
{
	// Squish JPEGs (strip exif, optimize coding) using jpegoptim: https://github.com/tjko/jpegoptim
	public class JpegOptimOptimizer : CommandLineToolOptimizer
	{
		protected override bool OptimizerUsesSeparateOutputFile => false;

		protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
		{
			return $"\"{tempFilePath}\"";
		}
	}
}
