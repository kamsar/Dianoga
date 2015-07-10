using System.IO;

namespace Dianoga.Png
{
	public class PngOptimizerResult : IOptimizerResult
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public int SizeBefore { get; set; }
		public int SizeAfter { get; set; }
		public byte[] OptimizedBytes { get; set; }

		public Stream CreateResultStream()
		{
			return new MemoryStream(OptimizedBytes);
		}
	}
}
