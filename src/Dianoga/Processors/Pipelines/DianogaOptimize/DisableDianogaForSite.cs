using Sitecore;
using Sitecore.Diagnostics;

namespace Dianoga.Processors.Pipelines.DianogaOptimize
{
    public class DisableDianogaForSite
    {
        public virtual void Process(ProcessorArgs args)
        {
            if (Context.Site?.Properties["enableDianoga"] == "false")
            {
                args.AbortPipeline();
            }
        }
    }
}
