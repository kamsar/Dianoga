using System;
using System.Collections.Generic;
using System.Linq;
using Dianoga.Optimizers;
using Sitecore.Pipelines;

namespace Dianoga.Processors.Pipelines.DianogaOptimize
{
	public class ExtensionBasedOptimizer : DianogaOptimizeProcessor
	{
		private HashSet<string> _supportedExtensionsLookup;

		public string Extensions
		{
			get { return string.Join(",", _supportedExtensionsLookup); }
			set { _supportedExtensionsLookup = new HashSet<string>(value.Split(',').Select(val => val.Trim(',', '.', '*', ' ')), StringComparer.OrdinalIgnoreCase); }
		}

		public string Pipeline { get; set; }

		protected override void ProcessOptimize(ProcessorArgs args)
		{
			if (_supportedExtensionsLookup.Contains(args.InputStream.Extension))
			{

				var sourceStream = args.ResultStream ?? args.InputStream.Stream;

				var optimizerArgs = new OptimizerArgs(sourceStream, args.MediaOptions);

				CorePipeline.Run(Pipeline, optimizerArgs);

				if (optimizerArgs.IsOptimized)
				{
					args.Extension = optimizerArgs.Extension;
					args.ResultStream = optimizerArgs.Stream;
				}

				if (!string.IsNullOrEmpty(optimizerArgs.Message))
				{
					args.AddMessage(optimizerArgs.Message);
				}

				if (optimizerArgs.Aborted)
				{
					args.AbortPipeline();
				}
			}
			
		}
	}
}
