using System.Web;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;

namespace Dianoga.Invokers.GetMediaStreamSync
{
	/// <summary>
	/// Optimizes images as they are served, before they go into media cache
	/// Advantage: optimized images always served
	/// Disadvantage: user must wait for optimization to complete if they do the first hit
	/// </summary>
	public class OptimizeImage
	{
		private readonly MediaOptimizer _optimizer;

		public OptimizeImage() : this(new MediaOptimizer())
		{

		}

		protected OptimizeImage(MediaOptimizer optimizer)
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

			MediaStream optimizedOutputStream = _optimizer.Process(outputStream, args.Options);

			if (optimizedOutputStream != null)
			{
				outputStream.Stream.Close();

				args.OutputStream = optimizedOutputStream;
			}
		}
	}
}