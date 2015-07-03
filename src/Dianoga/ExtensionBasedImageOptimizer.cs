using System;
using System.Linq;
using Sitecore.Resources.Media;

namespace Dianoga
{
	public abstract class ExtensionBasedImageOptimizer : IImageOptimizer
	{
		protected abstract string[] SupportedExtensions { get; }

		public virtual bool CanOptimize(MediaStream stream)
		{
			return SupportedExtensions.Any(ext => ext.Equals(stream.Extension, StringComparison.OrdinalIgnoreCase));
		}

		public abstract IOptimizerResult Optimize(MediaStream stream);
	}
}
