using Dianoga.NextGenFormats;

namespace Dianoga.Optimizers.Pipelines.DianogaJxl
{
	public class JxlOptimizer : CommandLineToolOptimizer
	{
		public readonly string Extension = "jxl";
		public bool DisableResizing { get; set; }

		public override void Process(OptimizerArgs args)
		{
			if (args.MediaOptions.CheckSupportOfExtension(Extension))
			{
				base.Process(args);

				if (args.IsOptimized)
				{
					args.Extension = Extension;

					//If Jpeg-XL optimization was executed then abort running other optimizers
					//because they don't accept jxl input file format
					args.AbortPipeline();
				}
			}
		}

		protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
		{
			return $"\"{tempFilePath}\" \"{tempOutputPath}\" ";
		}
	}
}
