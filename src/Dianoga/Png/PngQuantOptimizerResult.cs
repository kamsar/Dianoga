using System.IO;

namespace Dianoga.Png
{
	public class PngQuantOptimizerResult : IOptimizerResult
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
