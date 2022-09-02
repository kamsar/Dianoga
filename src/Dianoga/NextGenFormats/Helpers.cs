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

			var customAccept = context?.Request?.Headers?["customAccept"];
			if (!string.IsNullOrEmpty(customAccept))
				return PipelineHelpers.RunDianogaGetSupportedFormatsPipeline(new string[] { customAccept });

			return string.Empty;
		}

		public virtual bool CheckSupportedFormat(HttpContextBase context, string extension)
		{
			var acceptTypes = context?.Request?.AcceptTypes ?? new string[] { };
			if (acceptTypes.Any())
				return CheckSupportedFormat(string.Join(",", acceptTypes), extension);

			var customAccept = context?.Request?.Headers?["customAccept"];
			if (!string.IsNullOrEmpty(customAccept))
				return CheckSupportedFormat(customAccept, extension);

			return false;
		}

		public virtual bool CheckSupportedFormat(string input, string extension)
		{
			return input?.Contains(extension) ?? false;
		}

		public virtual string GetCustomOptions(HttpContextBase context)
		{
			var requestExtension = context?.Request.QueryString?["extension"];
			var customExtension = !string.IsNullOrEmpty(requestExtension) ? requestExtension : GetSupportedFormats(context);

			return customExtension;
		}
	}
}
