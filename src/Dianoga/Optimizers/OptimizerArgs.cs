using System.IO;
using Sitecore.Pipelines;
using Sitecore.Resources.Media;

namespace Dianoga.Optimizers
{
	public class OptimizerArgs : PipelineArgs
	{
		public Stream Stream { get; set; }

		public MediaOptions MediaOptions { get; }

		public bool IsOptimized { get; set; }

		public bool AcceptWebP { get; set; }

		public OptimizerArgs(Stream inputStream)
		{
			IsOptimized = false;
			Stream = inputStream;
		}

		public OptimizerArgs(Stream inputStream, MediaOptions options, bool acceptWebP) : this(inputStream)
		{
			MediaOptions = options;
			AcceptWebP = acceptWebP;
		}
	}
}
