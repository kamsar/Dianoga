using System.IO;

namespace Dianoga
{
	public interface IOptimizerResult
	{
		bool Success { get; set; }
		string ErrorMessage { get; set; }
		int SizeBefore { get; set; }
		int SizeAfter { get; set; }

		Stream CreateResultStream();
	}
}
