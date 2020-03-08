#if !NET452
using System.Linq;
using System.Web;
using Sitecore.Pipelines;
using Sitecore.XA.Foundation.MediaRequestHandler.Pipelines.MediaRequestHandler;

namespace Dianoga
{
	public class MediaRequestHandlerXA : Sitecore.XA.Foundation.MediaRequestHandler.MediaRequestHandler
	{
		protected override bool DoProcessRequest(HttpContext context)
		{
			var mediaRequestHandlerArgs = new MediaRequestHandlerArgs(context);
			CorePipeline.Run("mediaRequestHandler", mediaRequestHandlerArgs, failIfNotExists: false);
			if (mediaRequestHandlerArgs.Aborted)
			{
				return mediaRequestHandlerArgs.Result;
			}
			if (context?.Request.AcceptTypes != null && context.Request.AcceptTypes.Contains("image/webp"))
			{
				mediaRequestHandlerArgs.Request.Options.CustomOptions["extension"] = "webp";
			}
			return DoProcessRequest(mediaRequestHandlerArgs.Context, mediaRequestHandlerArgs.Request, mediaRequestHandlerArgs.Media);
		}
	}
}
#endif