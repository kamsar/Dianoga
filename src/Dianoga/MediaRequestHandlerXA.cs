namespace Dianoga
{
	using Sitecore.Pipelines;
	using Sitecore.XA.Foundation.MediaRequestHandler.Pipelines.MediaRequestHandler;
	using Sitecore.XA.Foundation.MediaRequestHandler.Pipelines.MediaRequestHeaders;
	using System.Linq;
	using System.Web;

	/// <summary>
	/// Media request handler for projects using SXA
	/// </summary>
	public class MediaRequestHandlerXA : Sitecore.XA.Foundation.MediaRequestHandler.MediaRequestHandler
	{
		protected override bool DoProcessRequest(HttpContext context)
		{
			MediaRequestHandlerArgs mediaRequestHandlerArgs = new MediaRequestHandlerArgs(context);
			CorePipeline.Run("mediaRequestHandler", mediaRequestHandlerArgs, failIfNotExists: false);
			if (mediaRequestHandlerArgs.Aborted)
			{
				return mediaRequestHandlerArgs.Result;
			}
			if ((context?.Request.AcceptTypes != null && context.Request.AcceptTypes.Contains("image/webp")) 
				|| Sitecore.Configuration.Settings.GetBoolSetting("Dianoga.WebP.ShowAlways", false))
			{
				mediaRequestHandlerArgs.Request.Options.CustomOptions["extension"] = "webp";
			}
			return DoProcessRequest(mediaRequestHandlerArgs.Context, mediaRequestHandlerArgs.Request, mediaRequestHandlerArgs.Media);
		}

		protected override void SendMediaHeaders(Sitecore.Resources.Media.Media media, HttpContext context)
		{
			base.SendMediaHeaders(media, context);
			CorePipeline.Run("mediaRequestHeaders", new MediaRequestHeadersArgs() { Media = media, Context = context }, false);
		}
	}
}
