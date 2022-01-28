using System;
using Dianoga.Invokers.GetMediaStreamSync;
using Dianoga.Invokers.MediaCacheAsync;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;

namespace Dianoga.Svg.Pipelines.GetMediaStream
{
	/// <summary>
	/// Ignores attempting to resize SVGs
	/// See https://jammykam.wordpress.com/2015/11/18/svg-in-media-library-polluting-log-files-with-errors/
	/// </summary>
	public class SvgIgnorer
	{
		public bool SynchronouslyOptimizeSvgs { get; set; }

		public void Process(GetMediaStreamPipelineArgs args)
		{
			Assert.ArgumentNotNull(args, "args");

			if (args.MediaData.MimeType.Equals("image/svg+xml", StringComparison.Ordinal))
			{
				// if the synchronous flag is set, the SVG will be optimized before we abort the pipeline to prevent
				// Sitecore from trying to process an SVG. Set this flag when NOT using the media cache async optimization strategy.
				if(SynchronouslyOptimizeSvgs && !(MediaManager.Cache is OptimizingMediaCache))
					new OptimizeImage().Process(args);

				args.AbortPipeline();
			}
		}
	}
}
