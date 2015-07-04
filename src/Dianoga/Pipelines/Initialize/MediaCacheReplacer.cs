using Sitecore.Pipelines;
using Sitecore.Resources.Media;

namespace Dianoga.Pipelines.Initialize
{
	public class MediaCacheReplacer
	{
		public virtual void Process(PipelineArgs args)
		{
			MediaManager.Cache = new OptimizingMediaCache(new MediaOptimizer());
		}
	}
}
