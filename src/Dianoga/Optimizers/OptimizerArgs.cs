using System.IO;
using Sitecore.Pipelines;

namespace Dianoga.Optimizers
{
	public class OptimizerArgs : PipelineArgs
	{
		public Stream Stream { get; set; }

		public bool IsOptimized { get; set; }

		public bool AcceptWebP { get; set; }

		public OptimizerArgs(Stream inputStream)
		{
			IsOptimized = false;
			Stream = inputStream;
		}

		public OptimizerArgs(Stream inputStream, bool acceptWebP) : this(inputStream)
		{
			AcceptWebP = acceptWebP;
		}
	}
}
