using System;
using System.Threading.Tasks.Dataflow;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Resources.Media;

namespace Dianoga.Invokers.MediaCacheAsync.Pipelines.Initialize
{
	public class MediaCacheReplacer
	{
		private int _maxConcurrentThreads = 1; // Hardcoded to 1 as we can see some concurrency issues with more threads

		public virtual void Process(PipelineArgs args)
		{
			var actionBlock = new ActionBlock<Action>(
				action => { action(); },
				new ExecutionDataflowBlockOptions
				{
					MaxDegreeOfParallelism = _maxConcurrentThreads
				}
			);

			MediaManager.Cache = new OptimizingMediaCache(new MediaOptimizer(), actionBlock);
			Log.Info($"Dianoga: Installed optimizing media cache to provide async optimization with max {_maxConcurrentThreads} concurrent threads.", this);
		}
	}
}
