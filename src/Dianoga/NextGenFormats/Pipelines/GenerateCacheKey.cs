using System.Web;
using Dianoga.NextGenFormats;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace Dianoga.WebP.Pipelines
{
	public class GenerateCacheKey : RenderRenderingProcessor
	{
		public virtual string Extension
		{
			get;
			set;
		}

		public override void Process(RenderRenderingArgs args)
		{
			Assert.ArgumentNotNull(args, nameof(args));
			if (args.Rendered || !args.Cacheable || !NextGenFormats.Helpers.CdnEnabled)
				return;

			var extensionSupport = new HttpContextWrapper(HttpContext.Current).CheckSupportOfExtension(Extension);
			var cacheKey = $"_#{Extension}:{extensionSupport}";
			args.CacheKey += cacheKey;
		}
	}
}
