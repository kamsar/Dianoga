using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Resources.Media;
using System;
using System.Threading.Tasks.Dataflow;

namespace Dianoga.Invokers.MediaCacheAsync.Pipelines.Initialize
{
	public class MediaCacheReplacer
	{
		private int _maxConcurrentThreads = -1;
		public int MaxConcurrentThreads
		{
			get => _maxConcurrentThreads;
			set => _maxConcurrentThreads = value < 1 ? -1 : value;
		}

		public virtual void Process(PipelineArgs args)
		{
			var actionBlock = new ActionBlock<Action>(
				action => { action(); },
				new ExecutionDataflowBlockOptions
				{
					MaxDegreeOfParallelism = MaxConcurrentThreads
				}
			);

			MediaManager.Cache = new OptimizingMediaCache(new MediaOptimizer(), actionBlock);
			Log.Info($"Dianoga: Installed optimizing media cache to provide async optimization with max {MaxConcurrentThreads} concurrent threads.", this);
		}
	}
}
