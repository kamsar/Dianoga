using System.IO;

namespace Dianoga.Jpeg
{
	public class JpegOptimizerResult : IOptimizerResult
	{
		public bool Success { get; internal set; }
		public string ErrorMessage { get; internal set; }
		public int SizeBefore { get; internal set; }
		public int SizeAfter { get; internal set; }
		public Stream ResultStream { get; internal set; }

		public Stream CreateResultStream()
		{
			return ResultStream;
		}
	}
}
