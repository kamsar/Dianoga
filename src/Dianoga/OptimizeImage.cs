using System;
using System.Diagnostics;
using Dianoga.Jpeg;
using Dianoga.Png;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.StringExtensions;

namespace Dianoga
{
	public class OptimizeImage
	{
		public void Process(GetMediaStreamPipelineArgs args)
		{
			Assert.ArgumentNotNull(args, "args");

			if (args.Options.Thumbnail) return;

			MediaStream outputStream = args.OutputStream;
			if (outputStream == null) return;

			if (!outputStream.AllowMemoryLoading)
			{
				Tracer.Error("Could not resize image as it was larger than the maximum size allowed for memory processing. Media item: {0}", outputStream.MediaItem.Path);
				return;
			}

			string mimeType = args.MediaData.MimeType;
			if (!mimeType.StartsWith("image/", StringComparison.Ordinal)) return;

			string extension = args.MediaData.Extension;

			IImageOptimizer optimizer = null;

			if (extension.Equals("png"))
			{
				optimizer = new PngOptimizer(outputStream.Stream);
			}

			if (extension.Equals("jpg") || extension.Equals("jpeg"))
			{
				optimizer = new JpegOptimizer(outputStream.Stream);
			}

			if (optimizer == null) return;

			var sw = new Stopwatch();
			sw.Start();

			var result = optimizer.Optimize();

			sw.Stop();

			if (result.Success)
			{
				outputStream.Stream.Close();

				Log.Info("Dianoga: optimized {0}.{1} ({2} bytes) - saved {3} bytes / {4:p}. Optimized in {5}ms.".FormatWith(args.OutputStream.MediaItem.MediaPath, args.OutputStream.MediaItem.Extension, result.SizeAfter, result.SizeBefore - result.SizeAfter, 1 - ((result.SizeAfter/(float) result.SizeBefore)), sw.ElapsedMilliseconds), this);

				args.OutputStream = new MediaStream(result.CreateResultStream(), outputStream.Extension, outputStream.MediaItem);
			}
			else
			{
				Log.Error("Dianoga: unable to optimize {0} because {1}".FormatWith(args.OutputStream.MediaItem.Name, result.ErrorMessage), this);
			}
		}
	}
}
