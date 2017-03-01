using System.IO;

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
			var rootPath = Path.GetDirectoryName(ExePath);

			return $"\"{rootPath}\\node_modules\\svgo\\bin\\svgo\" --input=\"{tempFilePath}\" --output=\"{tempOutputPath}\"";
		}

		protected override bool PrependAdditionalArguments => false;
	}
}
