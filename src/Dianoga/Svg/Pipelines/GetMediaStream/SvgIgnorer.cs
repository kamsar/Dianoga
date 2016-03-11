using System;
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
		public void Process(GetMediaStreamPipelineArgs args)
		{
			Assert.ArgumentNotNull(args, "args");
			if (args.MediaData.MimeType.Equals("image/svg+xml", StringComparison.Ordinal))
				args.AbortPipeline();
		}
	}
}
