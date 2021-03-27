#if !NET452
using System.Web;
using Dianoga.WebP;
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

			AddCustomOptions(context, mediaRequestHandlerArgs);

			return DoProcessRequest(mediaRequestHandlerArgs.Context, mediaRequestHandlerArgs.Request, mediaRequestHandlerArgs.Media);
		}

		protected virtual void AddCustomOptions(HttpContext context, MediaRequestHandlerArgs mediaRequestHandlerArgs)
		{
			var requestExtension = context?.Request.QueryString?["extension"];
			var supportsWebp = context.BrowserSupportsWebP();
			var supportsAvif = context.BrowserSupportsAvif();
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
					mediaRequestHandlerArgs.Request.Options.CustomOptions["extension"] = customExtension;
				}
			}
		}
	}
}
#endif