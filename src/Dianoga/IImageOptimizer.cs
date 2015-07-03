using Sitecore.Resources.Media;

namespace Dianoga
{
	public interface IImageOptimizer
	{
		bool CanOptimize(MediaStream stream);
		IOptimizerResult Optimize(MediaStream stream);
	}
}
