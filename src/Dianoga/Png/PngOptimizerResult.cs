using System.IO;

namespace Dianoga.Png
{
	public class PngOptimizerResult : IOptimizerResult
	{
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public int SizeBefore { get; internal set; }
		public int SizeAfter { get; internal set; }
		public byte[] OptimizedBytes { get; internal set; }

		public Stream CreateResultStream()
		{
			return new MemoryStream(OptimizedBytes);
		}
	}
}
