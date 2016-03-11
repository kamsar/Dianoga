namespace Dianoga.Optimizers.Pipelines.DianogaSvg
{
	/// <summary>
	/// Uses SVGO to optimize SVGs at runtime.
	/// Must install SVGO (and Node.js) globally in path for this to work.
	/// </summary>
	public class SvgoOptimizer : CommandLineToolOptimizer
	{
		protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
		{
			UseShellExecute = true;

			return $"--input=\"{tempFilePath}\" --output=\"{tempOutputPath}\"";
		}
	}
}
