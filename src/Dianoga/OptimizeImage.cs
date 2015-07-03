using Sitecore.Diagnostics;
using Sitecore.Resources.Media;

namespace Dianoga
{
	public class OptimizeImage
	{
		private readonly MediaOptimizer _optimizer;

		public OptimizeImage() : this(new MediaOptimizer())
		{
			
		}

		public OptimizeImage(MediaOptimizer optimizer)
		{
			Assert.ArgumentNotNull(optimizer, "optimizer");

			_optimizer = optimizer;
		}

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

			if (_optimizer.CanOptimize(outputStream))
			{
				MediaStream optimizedOutputStream = _optimizer.Process(outputStream, args.Options);

				if (optimizedOutputStream != null)
				{
					outputStream.Stream.Close();

					args.OutputStream = optimizedOutputStream;
				}
			}
		}
	}
}
