using Dianoga.NextGenFormats;
using Dianoga.WebP;

namespace Dianoga.Optimizers.Pipelines.DianogaWebP
{
	public class WebPOptimizer : CommandLineToolOptimizer
	{
		public readonly string Extension = "webp";
		public bool DisableResizing { get; set; }

		public override void Process(OptimizerArgs args)
		{
			if (args.MediaOptions.CheckSupportOfExtension(Extension))
			{
				base.Process(args);

				if (args.IsOptimized)
				{
					args.Extension = Extension;

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

		/// <summary>
		/// Generate resize parameters
		/// </summary>
		protected override string GetMediaSpecificArguments(OptimizerArgs args)
		{
			var transformationOptions = args.MediaOptions.GetTransformationOptions();
			if (!DisableResizing && transformationOptions.ContainsResizing())
			{
				return $"-resize {transformationOptions.Size.Width} {transformationOptions.Size.Height}";
			}

			return null;
		}
	}
}
