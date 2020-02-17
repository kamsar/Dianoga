using System.Linq;
using System.Web;
using Sitecore.Resources.Media;

namespace Dianoga
{
	public class MediaRequestHandler : Sitecore.Resources.Media.MediaRequestHandler
	{
		protected override bool DoProcessRequest(HttpContext context, MediaRequest request, Media media)
		{
			if (context?.Request.AcceptTypes != null && context.Request.AcceptTypes.Contains("image/webp"))
			{
				request.Options.CustomOptions["extension"] = "webp";
			}

			return base.DoProcessRequest(context, request, media);
		}

	}
}
