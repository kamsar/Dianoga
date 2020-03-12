using System.Web;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace Dianoga.WebP
{
#pragma warning disable CS0612 // Type or member is obsolete
	public class MediaProvider : Sitecore.Resources.Media.MediaProvider
	{

		public override string GetMediaUrl(MediaItem item, MediaUrlOptions options)
		{
			var url = base.GetMediaUrl(item, options);

			if (item.MimeType.StartsWith("image") && HttpContext.Current.BrowserSupportsWebP())
			{
				url += "&extension=webp";
			}

			return url;
		}

#if NET471
		public override string GetMediaUrl(MediaItem item, Sitecore.Links.UrlBuilders.MediaUrlBuilderOptions options)
		{
			var url = base.GetMediaUrl(item, options);

			if (item.MimeType.StartsWith("image") && HttpContext.Current.BrowserSupportsWebP())
			{
				url += "&extension=webp";
			}

			return url;
		}
#endif

	}
#pragma warning restore CS0612 // Type or member is obsolete
}
