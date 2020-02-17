using System;
using System.Collections.Generic;
using System.Linq;

namespace Dianoga.Processors.Pipelines.DianogaOptimize
{
	public class PathExclusion : DianogaOptimizeProcessor
	{
		private readonly List<string> _excludedPaths = new List<string>();

		public void AddExclusion(string mediaPath)
		{
			_excludedPaths.Add(mediaPath + "/");
		}

		protected override void ProcessOptimize(ProcessorArgs args)
		{
			if(_excludedPaths.Any(ignoredPath => (args.InputStream.MediaItem.MediaPath + "/").StartsWith(ignoredPath, StringComparison.OrdinalIgnoreCase)))
				args.AbortPipeline();
		}
	}
}
