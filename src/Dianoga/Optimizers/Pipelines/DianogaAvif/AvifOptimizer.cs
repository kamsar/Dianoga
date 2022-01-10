using Dianoga.NextGenFormats;

namespace Dianoga.Optimizers.Pipelines.DianogaAvif
{
	public class AvifOptimizer : CommandLineToolOptimizer
	{
		public readonly string Extension = "avif";

		public override void Process(OptimizerArgs args)
		{
			if (args.MediaOptions.CheckSupportOfExtension(Extension))
			{
				base.Process(args);

				if (args.IsOptimized)
				{
					args.Extension = Extension;

					//If avif optimization was executed then abort running other optimizers
					//because they don't accept avif input file format
					args.AbortPipeline();
				}
			}
		}

		protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
		{
			return $"\"{tempFilePath}\" -o \"{tempOutputPath}\" ";
		}
	}
}
