using System;
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

			if (!IsValidMimeType(stream)) return null;

			var optimizer = CreateOptimizer(stream);

			if (optimizer == null) return null;

			var sw = new Stopwatch();
			sw.Start();

			var result = optimizer.Optimize(stream);

			sw.Stop();

			if (result.Success)
			{
				stream.Stream.Close();

				Log.Info("Dianoga: optimized {0}.{1} ({2} bytes) - saved {3} bytes / {4:p}. Optimized in {5}ms.".FormatWith(stream.MediaItem.MediaPath, stream.MediaItem.Extension, result.SizeAfter, result.SizeBefore - result.SizeAfter, 1 - ((result.SizeAfter / (float)result.SizeBefore)), sw.ElapsedMilliseconds), this);

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

		protected virtual bool IsValidMimeType(MediaStream stream)
		{
			string mimeType = stream.MimeType;
			return mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
		}
	}
}
