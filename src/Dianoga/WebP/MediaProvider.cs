using System.Web;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using Sitecore.Web;

namespace Dianoga.WebP
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
				var queryStringExtension = string.Empty;
				var supportsWebp = HttpContext.Current.BrowserSupportsWebP();
				var supportsAvif = HttpContext.Current.BrowserSupportsAvif();
				if (supportsWebp && supportsAvif)
				{
					queryStringExtension = "webp,avif";
				}
				else if (supportsWebp)
				{
					queryStringExtension = "webp";
				}
				else if (supportsAvif)
				{
					queryStringExtension = "avif";
				}
				if (!string.IsNullOrEmpty(queryStringExtension))
				{
					return WebUtil.AddQueryString(url, "extension", queryStringExtension);
				}
			}
			return url;
		}

	}
#pragma warning restore CS0612 // Type or member is obsolete
}
