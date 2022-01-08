using Sitecore.Pipelines;

namespace Dianoga.NextGenFormats
{
	public class PipelineHelpers
	{
		public virtual string RunDianogaGetSupportedFormatsPipeline(string[] acceptTypes)
		{
			var nextGenFormats = new SupportedFormatsArgs()
			{
				Input = string.Join(",", acceptTypes),
				Prefix = "image/"
			};
			CorePipeline.Run("dianogaGetSupportedFormats", nextGenFormats);

			return string.Join(",", nextGenFormats.Extensions);
		}
	}
}
