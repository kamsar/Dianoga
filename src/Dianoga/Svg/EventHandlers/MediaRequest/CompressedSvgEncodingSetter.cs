using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Sitecore.Events;
using Sitecore.Resources.Media;

namespace Dianoga.Svg.EventHandlers.MediaRequest
{
	/// <summary>
	/// Tells the browser that we're sending gzipped media if the CompressStream handler has gzipped it.
	/// Based on Anders Laub's https://laubplusco.net/compress-svg-images-sitecore-media-library/
	/// </summary>
	public class CompressedSvgEncodingSetter
	{
		public void OnMediaRequest(object sender, EventArgs args)
		{
			var sitecoreEventArgs = (SitecoreEventArgs)args;

			if (sitecoreEventArgs == null || !sitecoreEventArgs.Parameters.Any())
				return;

			var request = (Sitecore.Resources.Media.MediaRequest)sitecoreEventArgs.Parameters[0];

			var media = MediaManager.GetMedia(request.MediaUri);

			if (!media.Extension.Equals("svg", StringComparison.OrdinalIgnoreCase))
				return;

			using (var stream = media.GetStream(request.Options))
			{
				// too short, cannot be gzip
				if (stream.Length < 3) return;

				var bytes = new byte[3];
				stream.Stream.Read(bytes, 0, 3);

				if (bytes[0] == 0x1f && bytes[1] == 0x8b && bytes[2] == 0x08) // this is the gzip format header http://stackoverflow.com/questions/19120676/how-to-detect-type-of-compression-used-on-the-file-if-no-file-extension-is-spe
					SetContentEncoding(request.InnerRequest.RequestContext.HttpContext.Response);
			}
		}

		private static void SetContentEncoding(HttpResponseBase response)
		{
			if (HasGzipContentEncoding(response.Headers))
				return;

			response.AddHeader("Content-encoding", "gzip");
		}

		private static bool HasGzipContentEncoding(NameValueCollection headers)
		{
			return headers.AllKeys.Any(k => k.Equals("content-encoding", StringComparison.InvariantCultureIgnoreCase))
			  && headers["content-encoding"].Equals("gzip", StringComparison.InvariantCultureIgnoreCase);
		}

	}
}
