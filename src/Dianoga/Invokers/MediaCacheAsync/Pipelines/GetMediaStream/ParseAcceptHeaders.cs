using System.Linq;
using System.Web;
using Sitecore.Resources.Media;

namespace Dianoga.Invokers.MediaCacheAsync.Pipelines.GetMediaStream
{
	public class ParseAcceptHeaders
	{
		private bool? _browserSupportWebP;

		public virtual bool BrowserSupportWebP
		{
			get
			{
				if (_browserSupportWebP != null)
				{
					return _browserSupportWebP.Value;
				}
				return (HttpContext.Current != null)
					   && (HttpContext.Current.Request.AcceptTypes != null)
					   && (HttpContext.Current.Request.AcceptTypes).Contains("image/webp");
			}
			set { _browserSupportWebP = value; }
		}

		public virtual void Process(GetMediaStreamPipelineArgs args)
		{
			if (BrowserSupportWebP)
			{
				args.Options.CustomOptions["extension"] = "webp";
			}
		}
	}
}
