namespace Dianoga.Optimizers.Pipelines.DianogaJpeg
{
	public class MozJpegOptimizer : CommandLineToolOptimizer
	{
		protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
		{
			return $"-outfile \"{tempOutputPath}\" \"{tempFilePath}\"";
		}
	}
}
