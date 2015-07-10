using System.IO;

namespace Dianoga.Jpeg
{
	public class JpegOptimizerResult : IOptimizerResult
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public int SizeBefore { get; set; }
		public int SizeAfter { get; set; }
		public Stream ResultStream { get; set; }

		public Stream CreateResultStream()
		{
			return ResultStream;
		}
	}
}
