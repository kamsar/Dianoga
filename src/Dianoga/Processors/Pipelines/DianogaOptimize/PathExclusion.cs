using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dianoga.Processors.Pipelines.DianogaOptimize
{
	public class PathExclusion : DianogaOptimizeProcessor
	{
		private readonly List<string> _excludedPaths = new List<string>();

		public void AddExclusion(string mediaPath)
		{
			_excludedPaths.Add(mediaPath);
		}

		protected override void ProcessOptimize(ProcessorArgs args)
		{
			var mediaPath = args.InputStream.MediaItem.MediaPath;
			if (IsExcluded(mediaPath))
			{
				args.AbortPipeline();
			}
		}

		public bool IsExcluded(string mediaPath)
		{
			mediaPath = mediaPath.ToLower();
			foreach (var path in _excludedPaths)
			{
				if (Regex.IsMatch(mediaPath, WildCardToRegular(path.ToLower())))
				{
					return true;
				}
			}
			return false;
		}

		private static string WildCardToRegular(String value)
		{
			return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
		}
	}
}
