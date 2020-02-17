using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Resources.Media;

namespace Dianoga.Invokers.MediaCacheAsync.Pipelines.Initialize
{
	public class MediaCacheReplacer
	{
		public virtual void Process(PipelineArgs args)
		{
			MediaManager.Cache = new OptimizingMediaCache(new MediaOptimizer());
			Log.Info("Dianoga: Installed optimizing media cache to provide async optimization.", this);
		}
	}
}
