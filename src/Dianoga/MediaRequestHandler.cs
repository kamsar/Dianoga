using System.Web;
using Dianoga.WebP;
using Sitecore.Resources.Media;

namespace Dianoga
{
	public class MediaRequestHandler : Sitecore.Resources.Media.MediaRequestHandler
	{
		protected override bool DoProcessRequest(HttpContext context, MediaRequest request, Media media)
		{
			var supportsWebp = context.BrowserSupportsWebP();
			var supportsAvif = context.BrowserSupportsAvif();
			var requestExtension = context?.Request.QueryString?["extension"];

			if ((requestExtension?.Contains("webp") ?? false) ||
			    (requestExtension?.Contains("avif") ?? false) ||
			    (!Helpers.CdnEnabled && (supportsWebp || supportsAvif)))
			{
				var customExtension = string.Empty;
				if (supportsWebp && supportsAvif)
				{
					customExtension = "webp,avif";
				}
				else if (supportsWebp)
				{
					customExtension = "webp";
				}
				else if (supportsAvif)
				{
					customExtension = "avif";
				}
				if (!string.IsNullOrEmpty(customExtension))
				{
					request.Options.CustomOptions["extension"] = customExtension;
				}
			}

			return base.DoProcessRequest(context, request, media);
		}

	}
}
