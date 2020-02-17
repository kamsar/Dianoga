using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Resources.Media;

namespace Dianoga.Helpers
{
    public static class MediaOptionsHelpers
    {
        public static bool BrowserSupportsWebP(this MediaOptions mediaOptions)
        {
            return mediaOptions.GetCustomExtension() == "webp";
        }

        public static string GetCustomExtension(this MediaOptions mediaOptions)
        {
            return mediaOptions.CustomOptions["extension"];
        }
    }
}
