using System.Web;
using Sitecore.Resources.Media;

namespace Dianoga.NextGenFormats
{
	public class MediaRequestHandler : Sitecore.Resources.Media.MediaRequestHandler
	{
		protected override bool DoProcessRequest(HttpContext context, MediaRequest request, Media media)
		{
			request.AddCustomOptions(new HttpContextWrapper(context));
			return base.DoProcessRequest(context, request, media);
		}
	}
}
