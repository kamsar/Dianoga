namespace Dianoga.Optimizers.Pipelines.DianogaJpeg
{
	// Squish JPEGs (strip exif, optimize coding) using jpegtran: http://jpegclub.org/jpegtran/
	public class MozJpegOptimizer : CommandLineToolOptimizer
	{
		protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
		{
			return $"-optimise -copy none -outfile \"{tempOutputPath}\" \"{tempFilePath}\"";
		}
	}
}
