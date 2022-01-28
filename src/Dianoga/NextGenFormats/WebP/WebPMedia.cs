using Sitecore.Resources.Media;

namespace Dianoga.NextGenFormats.WebP
{
	public class WebPMedia : ImageMedia
	{
		/// <summary>The clone.</summary>
		/// <returns>
		/// The <see cref="T:Sitecore.Resources.Media.Media" />.
		/// </returns>
		public override Sitecore.Resources.Media.Media Clone()
		{
			return new WebPMedia();
		}

		/// <summary> The update meta data. </summary>
		/// <param name="mediaStream">The media stream.</param>
		protected override void UpdateImageMetaData(MediaStream mediaStream)
		{
			// TODO: set default metadata for height and width
		}
	}
}
