using System.IO;

namespace Dianoga
{
	public interface IOptimizerResult
	{
		bool Success { get; }
		string ErrorMessage { get; }
		int SizeBefore { get; }
		int SizeAfter { get; }

		Stream CreateResultStream();
	}
}
