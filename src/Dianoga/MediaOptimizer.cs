using System.Diagnostics;
using System.Linq;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.StringExtensions;

namespace Dianoga
{
	public class MediaOptimizer
	{
		protected static readonly IImageOptimizer[] Optimizers;

		static MediaOptimizer()
		{
			var config = Factory.GetConfigNode("/sitecore/dianoga/optimizers");
			Assert.IsNotNull(config, "Missing dianoga/optimizers config node! Missing or outdated App_Config/Include/Dianoga.config?");

			Optimizers = config.ChildNodes
				.OfType<XmlElement>()
				.Select(Factory.CreateObject<IImageOptimizer>)
				.ToArray();
		}

		public virtual MediaStream Process(MediaStream stream, MediaOptions options)
		{
			Assert.ArgumentNotNull(stream, "stream");

			if (!stream.AllowMemoryLoading)
			{
				Tracer.Error("Could not resize image as it was larger than the maximum size allowed for memory processing. Media item: {0}", stream.MediaItem.Path);
				return null;
			}

			var optimizer = CreateOptimizer(stream);

			if (optimizer == null) return null;

			var sw = new Stopwatch();
			sw.Start();

			var result = optimizer.Optimize(stream);

			sw.Stop();

			if (result.Success)
			{
				stream.Stream.Close();

				Log.Info("Dianoga: optimized {0}.{1} [{2}] (final size: {3} bytes) - saved {4} bytes / {5:p}. Optimized in {6}ms.".FormatWith(stream.MediaItem.MediaPath, stream.MediaItem.Extension, GetDimensions(options), result.SizeAfter, result.SizeBefore - result.SizeAfter, 1 - ((result.SizeAfter / (float)result.SizeBefore)), sw.ElapsedMilliseconds), this);

				return new MediaStream(result.CreateResultStream(), stream.Extension, stream.MediaItem);
			}
			
			Log.Error("Dianoga: unable to optimize {0} because {1}".FormatWith(stream.MediaItem.Name, result.ErrorMessage), this);

			return null;
		}

		public virtual bool CanOptimize(MediaStream stream)
		{
			return CreateOptimizer(stream) != null;
		}

		protected virtual IImageOptimizer CreateOptimizer(MediaStream stream)
		{
			return Optimizers.FirstOrDefault(optimizer => optimizer.CanOptimize(stream));
		}

		protected virtual string GetDimensions(MediaOptions options)
		{
			if (options.MaxHeight == 0 && options.MaxWidth == 0 && options.Height == 0 && options.Width == 0) return "original size";
			
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
