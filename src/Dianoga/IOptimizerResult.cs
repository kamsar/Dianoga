using System.IO;

namespace Dianoga
{
	public interface IOptimizerResult
	{
		bool Success { get; set; }
		string ErrorMessage { get; set; }
		int SizeBefore { get; }
		int SizeAfter { get; }

		Stream CreateResultStream();
	}
}
