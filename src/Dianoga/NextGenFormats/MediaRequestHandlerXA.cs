#if !NET452
using System.Web;
using Sitecore.Pipelines;
using Sitecore.XA.Foundation.MediaRequestHandler.Pipelines.MediaRequestHandler;

namespace Dianoga.NextGenFormats
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

			mediaRequestHandlerArgs.Request.AddCustomOptions(new HttpContextWrapper(context));

			return DoProcessRequest(mediaRequestHandlerArgs.Context, mediaRequestHandlerArgs.Request, mediaRequestHandlerArgs.Media);
		}
	}
}
#endif