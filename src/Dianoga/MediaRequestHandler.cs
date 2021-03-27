using System.Web;
using Dianoga.WebP;
using Sitecore.Resources.Media;

namespace Dianoga
{
	public class MediaRequestHandler : Sitecore.Resources.Media.MediaRequestHandler
	{
		protected override bool DoProcessRequest(HttpContext context, MediaRequest request, Media media)
		{
			if ((context?.Request.QueryString?["extension"]?.Contains("webp") ?? false) || (!Helpers.CdnEnabled && context.BrowserSupportsWebP()))
			{
				request.Options.CustomOptions["extension"] = "webp";
			}

			return base.DoProcessRequest(context, request, media);
		}

	}
}
