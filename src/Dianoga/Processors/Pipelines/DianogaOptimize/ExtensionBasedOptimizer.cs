using System;
using System.Collections.Generic;
using System.IO;
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
				// make sure we can buffer the media stream if we're going to act on it
				if (args.ResultStream == null)
				{
					args.InputStream.MakeStreamSeekable();
					args.InputStream.Stream.Seek(0, SeekOrigin.Begin);
				}

				var sourceStream = args.ResultStream ?? args.InputStream.Stream;

				var optimizerArgs = new OptimizerArgs(sourceStream, args.AcceptWebP);

				CorePipeline.Run(Pipeline, optimizerArgs);

				if (optimizerArgs.IsOptimized)
					args.ResultStream = optimizerArgs.Stream;

				if(optimizerArgs.Aborted)
					args.AbortPipeline();
			}
		}
	}
}
