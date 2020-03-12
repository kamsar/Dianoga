using System.Web;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace Dianoga.WebP.Pipelines
{
	public class GenerateCacheKey : RenderRenderingProcessor
	{
		public override void Process(RenderRenderingArgs args)
		{
			Assert.ArgumentNotNull(args, nameof(args));
			if (args.Rendered || !args.Cacheable)
				return;

			var webp = HttpContext.Current.BrowserSupportsWebP();
			var cacheKey = "_#webp:" + webp;
			args.CacheKey += cacheKey;
		}
	}
}
