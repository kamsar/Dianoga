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
				if (args.ResultStream == null)
				{
					// MakeStreamSeekable will buffer the stream if its not seekable
					args.InputStream.MakeStreamSeekable();
					args.InputStream.Stream.Seek(0, SeekOrigin.Begin);
				}

				var sourceStream = args.ResultStream ?? args.InputStream.Stream;

				var optimizerArgs = new OptimizerArgs(sourceStream, args.MediaOptions, args.InputStream.MediaItem.MediaPath);

				CorePipeline.Run(Pipeline, optimizerArgs);

				args.IsOptimized = optimizerArgs.IsOptimized;
				args.Extension = optimizerArgs.Extension;
				args.ResultStream = optimizerArgs.Stream;

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
