using System;
using System.Diagnostics;
using Dianoga.Processors;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Resources.Media;

namespace Dianoga
{
	public class MediaOptimizer
	{
		/// <summary>
		/// Optimizes a media stream and returns the optimized result. The original stream is closed if processing is successful.
		/// Returns null if processing was unsuccessful.
		/// </summary>
		public virtual MediaStream Process(MediaStream stream, MediaOptions options)
		{
			Assert.ArgumentNotNull(stream, nameof(stream));
			Assert.ArgumentNotNull(options, nameof(options));

			if (!stream.AllowMemoryLoading)
			{
				Log.Error($"Dianoga: Could not resize image as it was larger than the maximum size allowed for memory processing. Media item: {stream.MediaItem.Path}", this);
				return null;
			}

			//Run optimizer based on extension
			var sw = new Stopwatch();
			sw.Start();

			var result = new ProcessorArgs(stream, options);

			try
			{
				CorePipeline.Run("dianogaOptimize", result);
			}
			catch (Exception exception)
			{
				Log.Error($"Dianoga: Unable to optimize {stream.MediaItem.MediaPath} due to a processing error! It will be unchanged.", exception, this);
				return null;
			}
			sw.Stop();

			if (result.ResultStream != null && result.ResultStream.CanRead)
			{
				if (result.Message.Length > 0)
				{
					Log.Info($"Dianoga: messages occurred while optimizing {stream.MediaItem.MediaPath}: {result.Message}", this);
				}

				var extension = result.Extension ?? stream.Extension;
				Log.Info($"Dianoga: optimized {stream.MediaItem.MediaPath}.{stream.MediaItem.Extension} [original size: {GetDimensions(options)} {result.Statistics.SizeBefore} bytes] [final size: {result.Statistics.SizeAfter} bytes] [saved {result.Statistics.BytesSaved} bytes / {result.Statistics.PercentageSaved:p}] [Optimized in {sw.ElapsedMilliseconds}ms] [Extension {extension}]", this);

				stream.Dispose();

				return new MediaStream(result.ResultStream, extension, stream.MediaItem);
			}

			if (!string.IsNullOrWhiteSpace(result.Message))
				Log.Warn($"Dianoga: unable to optimize {stream.MediaItem.MediaPath}.{stream.MediaItem.Extension} because {result.Message}", this);

			// if no message exists that implies that nothing in the dianogaOptimize pipeline acted to optimize - e.g. it's a media type we don't know how to optimize, like PDF.

			return null;
		}

		protected virtual string GetDimensions(MediaOptions options)
		{
			if (options.MaxHeight == 0 && options.MaxWidth == 0 && options.Height == 0 && options.Width == 0) return "";

			string result = string.Empty;

			if (options.Width > 0) result = options.Width + "w";
			else if (options.MaxWidth > 0) result = options.MaxWidth + "mw";

			if (result.Length > 0 && (options.Height > 0 || options.MaxHeight > 0))
			{
				result += " x ";
			}

			if (options.Height > 0) result += options.Height + "h";
			else if (options.MaxHeight > 0) result += options.MaxHeight + "mh";

			if (options.Thumbnail) result += " (thumb)";

			return result;
		}
	}
}
