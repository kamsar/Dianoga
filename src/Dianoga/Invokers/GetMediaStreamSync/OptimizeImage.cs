﻿using Sitecore.Diagnostics;
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
			if (Sitecore.Context.Site?.Name == "shell") return;

			MediaStream outputStream = args.OutputStream;
			if (outputStream == null) return;

			if (!outputStream.AllowMemoryLoading)
			{
				Tracer.Error("Could not resize image as it was larger than the maximum size allowed for memory processing. Media item: {0}", outputStream.MediaItem.Path);
				return;
			}

			MediaStream optimizedOutputStream = _optimizer.Process(outputStream, args.Options);

			if (optimizedOutputStream != null && outputStream.Stream != optimizedOutputStream.Stream)
			{
				outputStream.Dispose(); // Uses thread safe dispose helper that won't double dispose

				args.OutputStream = optimizedOutputStream;

				if (optimizedOutputStream.Extension == "webp" || optimizedOutputStream.Extension == "avif")
				{
					// Further processing will fail
					args.AbortPipeline();
				}
			}
			else
			{
				var mediaPath = outputStream.MediaItem.MediaPath;
				DianogaLog.Info($"Dianoga: {mediaPath} cannot be optimized due to media type or path exclusion");
			}
		}
	}
}