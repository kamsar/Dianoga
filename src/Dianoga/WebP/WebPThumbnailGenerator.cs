using Dianoga.Optimizers;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Resources.Media;

namespace Dianoga.WebP
{
	public class WebPThumbnailGenerator : ThumbnailGenerator
	{
		public override MediaStream GetStream(
		 MediaData mediaData,
		 TransformationOptions options)
		{
			Assert.ArgumentNotNull((object)mediaData, nameof(mediaData));
			Assert.ArgumentNotNull((object)options, "transformationOptions");
			MediaStream stream = mediaData.GetStream();
			if (stream == null)
				return (MediaStream)null;
			options = this.GetOptions(mediaData, options);
			using (stream)
			{
				return this.GetImageStream(stream, options);
			}
		}

		private MediaStream GetImageStream(MediaStream stream, TransformationOptions options)
		{
			Assert.ArgumentNotNull((object)stream, nameof(stream));
			Assert.ArgumentNotNull((object)options, nameof(options));
			var mediaOptions = new MediaOptions()
			{
				AllowStretch = options.AllowStretch,
				BackgroundColor = options.BackgroundColor,
				IgnoreAspectRatio = options.IgnoreAspectRatio,
				Scale = options.Scale,
				Width = options.Size.Width,
				Height = options.Size.Height,
				MaxWidth = options.MaxSize.Width,
				MaxHeight = options.MaxSize.Height
			};
			mediaOptions.CustomOptions["extension"] = "webp";
			var args = new OptimizerArgs(stream.Stream, mediaOptions, stream.MediaItem.MediaPath);
			CorePipeline.Run("dianogaOptimizeWebP", args);

			if (args.IsOptimized)
			{
				return new MediaStream(args.Stream, args.Extension, stream.MediaItem);
			}
			return null;

		}

		private TransformationOptions GetOptions(MediaData mediaData, TransformationOptions options)
		{
			options = options.Clone();
			if (options.Size.IsEmpty && (double)options.Scale == 0.0)
				options.Size = MediaManager.Config.GetThumbnailSize(mediaData.Extension);
			options.PreserveResolution = false;
			return options;
		}
	}
}
