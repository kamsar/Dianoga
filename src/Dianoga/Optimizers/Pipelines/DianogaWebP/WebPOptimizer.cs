using Dianoga.WebP;

namespace Dianoga.Optimizers.Pipelines.DianogaWebP
{
	public class WebPOptimizer : CommandLineToolOptimizer
	{
		private string _originalAdditionalToolArguments;

		public override void Process(OptimizerArgs args)
		{

			if (args.MediaOptions.BrowserSupportsWebP())
			{
				if (string.IsNullOrEmpty(_originalAdditionalToolArguments))
				{
					_originalAdditionalToolArguments = AdditionalToolArguments;
				}

				var transformationOptions = args.MediaOptions.GetTransformationOptions();
				if (!DisableResizing && transformationOptions.ContainsResizing())
				{
					AdditionalToolArguments = $"{_originalAdditionalToolArguments} -resize {transformationOptions.Size.Width} {transformationOptions.Size.Height}";
				}
				else
				{
					AdditionalToolArguments = _originalAdditionalToolArguments;
				}

				base.Process(args);

				if (args.IsOptimized)
				{
					args.Extension = "webp";

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
