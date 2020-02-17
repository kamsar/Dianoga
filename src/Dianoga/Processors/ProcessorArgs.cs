using Sitecore.Pipelines;
using System.IO;
using Sitecore.Resources.Media;

namespace Dianoga.Processors
{
	public class ProcessorArgs : PipelineArgs
	{
		public MediaStream InputStream { get; }

		public Stream ResultStream { get; set; }

		public ProcessorArgsStatistics Statistics { get; }

		public bool AcceptWebP { get; set; }

		public ProcessorArgs(MediaStream inputStream)
		{
			InputStream = inputStream;
			Statistics = new ProcessorArgsStatistics(this);
		}

		public ProcessorArgs(MediaStream inputStream, bool acceptWebP): this(inputStream)
		{
			AcceptWebP = acceptWebP;
		}


		public class ProcessorArgsStatistics
		{
			private readonly ProcessorArgs _args;

			internal ProcessorArgsStatistics(ProcessorArgs args)
			{
				_args = args;
				SizeBefore = _args.InputStream.Length;
			}

			public long SizeBefore { get; }
			public long SizeAfter => _args.ResultStream?.Length ?? SizeBefore;
			public float PercentageSaved => 1 - ((SizeAfter/(float) SizeBefore));
			public long BytesSaved => SizeBefore - SizeAfter;
		}
	}
}
