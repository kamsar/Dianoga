using System.Linq;
using System.Web;
using Sitecore.Resources.Media;

namespace Dianoga.WebP.Pipelines.GetMediaStream
{
	public class ParseWebPAcceptHeaders
	{
		public virtual void Process(GetMediaStreamPipelineArgs args)
		{
			if ((HttpContext.Current != null)
					&& (HttpContext.Current.Request.AcceptTypes != null)
					&& (HttpContext.Current.Request.AcceptTypes).Contains("image/webp"))
			{
				args.Options.CustomOptions["extension"] = "webp";
			}
		}
	}
}
