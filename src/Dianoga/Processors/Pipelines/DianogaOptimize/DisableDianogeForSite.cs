using Sitecore;

namespace Dianoga.Processors.Pipelines.DianogaOptimize
{
	public class DisableDianogeForSite
	{
		public virtual void Process(ProcessorArgs args)
		{
			if (Context.Site.Properties["enableDianoga"] == "false")
			{
				args.AbortPipeline();
			}
		}
	}
}