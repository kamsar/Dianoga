using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Resources.Media;
using Sitecore.Web;

namespace Dianoga.NextGenFormats
{
#pragma warning disable CS0612 // Type or member is obsolete
	public class MediaProvider : Sitecore.Resources.Media.MediaProvider
	{

		public override string GetMediaUrl(MediaItem item, MediaUrlOptions options)
		{
			var url = base.GetMediaUrl(item, options);

			url = GetMediaUrl(item, url);

			return url;
		}

#if NET471 || NET48
		public override string GetMediaUrl(MediaItem item, Sitecore.Links.UrlBuilders.MediaUrlBuilderOptions options)
		{
			var url = base.GetMediaUrl(item, options);

			url = GetMediaUrl(item, url);

			return url;
		}
#endif

		protected virtual string GetMediaUrl(MediaItem item, string url)
		{
			if (item.MimeType.StartsWith("image") && !url.Contains("extension"))
			{
				var acceptTypes = HttpContext.Current?.Request?.AcceptTypes ?? new string[]{};
				if (acceptTypes.Any())
				{
					var nextGenFormats = new SupportedFormatsArgs()
					{
						Input = string.Join(",", acceptTypes),
						Suffix = "image/"
					};
					CorePipeline.Run("dianogaGetSupportedFormats", nextGenFormats);

					var extensions = string.Join(",", nextGenFormats.Extensions);
					return WebUtil.AddQueryString(url, "extension", extensions);
				}
			}
			return url;
		}

	}
#pragma warning restore CS0612 // Type or member is obsolete
}
