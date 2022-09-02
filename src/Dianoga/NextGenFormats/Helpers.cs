using System.Linq;
using System.Web;

namespace Dianoga.NextGenFormats
{
	public class Helpers
	{
		public virtual PipelineHelpers PipelineHelpers { get; set; } = new PipelineHelpers();
		public virtual string CustomAcceptHeaderName { get; set; } = Sitecore.Configuration.Settings.GetSetting("Dianoga.CDN.CustomAcceptHeaderName");

		public static bool CdnEnabled => Sitecore.Configuration.Settings.GetBoolSetting("Dianoga.CDN.Enabled", false);

		public virtual string GetSupportedFormats(HttpContextBase context)
		{
			var acceptTypes = context?.Request?.AcceptTypes ?? new string[] { };
			if (acceptTypes.Any())
			{
				return PipelineHelpers.RunDianogaGetSupportedFormatsPipeline(acceptTypes);
			}

			if (!string.IsNullOrEmpty(CustomAcceptHeaderName))
			{
				var customAcceptHeader = context?.Request?.Headers?[CustomAcceptHeaderName];
				if (!string.IsNullOrEmpty(customAcceptHeader))
				{
					return PipelineHelpers.RunDianogaGetSupportedFormatsPipeline(new string[] { customAcceptHeader });
				}
			}

			return string.Empty;
		}

		public virtual bool CheckSupportedFormat(HttpContextBase context, string extension)
		{
			var acceptTypes = context?.Request?.AcceptTypes ?? new string[] { };
			if (acceptTypes.Any())
			{
				return CheckSupportedFormat(string.Join(",", acceptTypes), extension);
			}

			if (!string.IsNullOrEmpty(CustomAcceptHeaderName))
			{
				var customAcceptHeader = context?.Request?.Headers?[CustomAcceptHeaderName];
				if (!string.IsNullOrEmpty(customAcceptHeader))
				{
					return CheckSupportedFormat(customAcceptHeader, extension);
				}
			}

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
