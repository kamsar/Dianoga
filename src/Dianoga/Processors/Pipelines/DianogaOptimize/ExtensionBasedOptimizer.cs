using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pipelines;

namespace Dianoga.Processors.Pipelines.DianogaOptimize
{
	public abstract class ExtensionBasedOptimizer : DianogaOptimizeProcessor
	{
		private HashSet<string> _supportedExtensionsLookup;

		public string Extensions
		{
			get { return string.Join(",", _supportedExtensionsLookup); }
			set { _supportedExtensionsLookup = new HashSet<string>(value.Split(',').Select(val => val.Trim(',', '.', '*', ' ')), StringComparer.OrdinalIgnoreCase); }
		}

		public string PipelineName { get; set; }

		protected override void ProcessOptimize(ProcessorArgs args)
		{
			if (_supportedExtensionsLookup.Contains(args.InputStream.Extension))
			{
				CorePipeline.Run(PipelineName, args);
			}
		}
	}
}
