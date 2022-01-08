using System.Linq;
using System.Web;

namespace Dianoga.NextGenFormats
{
	public class Helpers
	{
		public virtual PipelineHelpers PipelineHelpers { get; set; } = new PipelineHelpers();

		public static bool CdnEnabled => Sitecore.Configuration.Settings.GetBoolSetting("Dianoga.CDN.Enabled", false);

		public virtual string GetSupportedFormats(HttpContextBase context)
		{
			var acceptTypes = context?.Request?.AcceptTypes ?? new string[] { };
			if (acceptTypes.Any())
			{
				return PipelineHelpers.RunDianogaGetSupportedFormatsPipeline(acceptTypes);
			}

			return string.Empty;
		}

		public virtual bool CheckSupportedFormat(HttpContextBase context, string extension)
		{
			var acceptTypes = context?.Request?.AcceptTypes ?? new string[] { };
			return acceptTypes.Any() && CheckSupportedFormat(string.Join(",", acceptTypes), extension);
		}

		public virtual bool CheckSupportedFormat(string input, string extension)
		{
			return input.Contains(extension);
		}

		public virtual string GetCustomOptions(HttpContextBase context)
		{
			var requestExtension = context?.Request.QueryString?["extension"];
			var customExtension = !string.IsNullOrEmpty(requestExtension) ? requestExtension : GetSupportedFormats(context);

			return customExtension;
		}
	}
}
