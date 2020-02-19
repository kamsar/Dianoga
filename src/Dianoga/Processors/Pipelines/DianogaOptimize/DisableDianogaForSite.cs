using Sitecore;
using Sitecore.Diagnostics;

namespace Dianoga.Processors.Pipelines.DianogaOptimize
{
    public class DisableDianogaForSite
    {
        public virtual void Process(ProcessorArgs args)
        {
            if (Context.Site?.Properties == null)
            {
                Log.Warn($"Dianoga: DisableDianogaForSite - null Context.Site.Properties - {args.InputStream?.MediaItem?.Path}", this);
                args.AbortPipeline();
            }
            if (Context.Site.Properties["enableDianoga"] == "false")
            {
                args.AbortPipeline();
            }
        }
    }
}
