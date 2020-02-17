using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Resources.Media;
using Sitecore.Rules.Conditions;

namespace Dianoga
{
	public class MediaRequestHandler : Sitecore.Resources.Media.MediaRequestHandler
	{
		protected override bool DoProcessRequest(HttpContext context, MediaRequest request, Media media)
		{
			if (context?.Request.AcceptTypes != null && (context.Request.AcceptTypes).Contains("image/webp"))
			{
				request.Options.CustomOptions["extension"] = "webp";
			}

			return base.DoProcessRequest(context, request, media);
		}

		private static bool AcceptWebP(HttpContext context)
		{
			return context?.Request.AcceptTypes != null && (context.Request.AcceptTypes).Contains("image/webp");
		}

	}
}
