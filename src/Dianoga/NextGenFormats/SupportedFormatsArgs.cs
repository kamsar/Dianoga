using Sitecore.Collections;
using Sitecore.Pipelines;

namespace Dianoga.NextGenFormats
{
	public class SupportedFormatsArgs : PipelineArgs
	{
		public SupportedFormatsArgs()
		{
			Extensions = new Set<string>();
		}
		public Set<string> Extensions { get; set; }
		public string Input { get; set; }
		public string Suffix { get; set; }
	}
}
