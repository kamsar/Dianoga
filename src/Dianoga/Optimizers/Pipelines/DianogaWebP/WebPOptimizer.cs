namespace Dianoga.Optimizers.Pipelines.DianogaWebP
{
	public class WebPOptimizer : CommandLineToolOptimizer
	{
		public override void Process(OptimizerArgs args)
		{

			if (args.AcceptWebP)
			{
				var transformationOptions = args.MediaOptions.GetTransformationOptions();
				if (transformationOptions.ContainsResizing())
				{
					this.AdditionalToolArguments = $"-resize {transformationOptions.Size.Width} {transformationOptions.Size.Height}";
				}

				base.Process(args);

				if (args.IsOptimized)
				{
					//If WebP optimization was executed then abort running other optimizers
					//because they don't accept webp input file format
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
