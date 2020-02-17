using System.IO;
using System.IO.Compression;

namespace Dianoga.Optimizers.Pipelines.DianogaSvg
{
	public class GzipSvgData : OptimizerProcessor
	{
		protected override void ProcessOptimizer(OptimizerArgs args)
		{
			var compressed = Compress(args.Stream);

			// dispose of the old output stream now that we've gzipped it
			args.Stream.Dispose();

			args.Stream = compressed;
			args.IsOptimized = true;
		}

		protected virtual Stream Compress(Stream input)
		{
			var compressedStream = new MemoryStream();

			using (var gzip = new GZipStream(compressedStream, CompressionMode.Compress, true))
			{
				input.CopyTo(gzip);
			}

			return compressedStream;
		}
	}
}