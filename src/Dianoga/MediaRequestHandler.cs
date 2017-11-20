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
        private HashSet<string> _webpProcessedExtensions;

        protected override bool DoProcessRequest(HttpContext context, MediaRequest request, Media media)
        {
            if (AcceptWebP(context))
            {
                request.Options.CustomOptions["extension"] = "webp";
            }

            var baseReturn = base.DoProcessRequest(context, request, media);

            //Without this part content-Type is application/octet-stream
            //ToDo: get rid from this part, because it works with "application/octet-stream" either cache _webpProcessedExtensions to avoid execution for each media request
            if (AcceptWebP(context))
            {
                _webpProcessedExtensions = new HashSet<string>(Sitecore.Configuration.Settings.GetSetting("Media.WebP.ProcessingExtensions").Split(',').Select(val => val.Trim(',', '.', '*', ' ')));
                if (_webpProcessedExtensions.Contains(media.Extension))
                {
                    context.Response.ContentType = "image/webp";
                }
            }
            return baseReturn;
        }

        private static bool AcceptWebP(HttpContext context)
        {
            return context?.Request.AcceptTypes != null && (context.Request.AcceptTypes).Contains("image/webp");
        }

    }
}
