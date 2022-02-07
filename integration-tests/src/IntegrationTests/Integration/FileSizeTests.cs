using FluentAssertions;
using Xunit;

namespace Integration
{
	public class FileSizeTests : BaseTests
	{
		[Theory]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg00.jpg", 4427080)]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg01.jpg", 4822280)]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg02.jpg", 2261738)]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg03.jpg", 5684535)]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg04.jpg", 3274103)]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg05.jpg", 5652302)]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg06.jpg", 5220868)]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg07.jpg", 1403617)]
		public void JpegSizeTest(string url, int size)
		{
			var wc = new System.Net.WebClient();
			var bytes = wc.DownloadData(CDHostname + url);
			bytes.Length.Should().BeLessThan(size, "Dianoga should squeeze JPEG image");
		}

		[Theory]
		[InlineData("/-/media/Project/Dianoga/Test/png/png00.png", 100825)]
		[InlineData("/-/media/Project/Dianoga/Test/png/png01.png", 277679)]
		[InlineData("/-/media/Project/Dianoga/Test/png/png02.png", 250645)]
		[InlineData("/-/media/Project/Dianoga/Test/png/png03.png", 110259)]
		[InlineData("/-/media/Project/Dianoga/Test/png/png04.png", 114270)]
		[InlineData("/-/media/Project/Dianoga/Test/png/png05.png", 56132)]
		public void PngSizeTest(string url, int size)
		{
			var wc = new System.Net.WebClient();
			var bytes = wc.DownloadData("https://cd.dockerexamples.localhost/" + url);
			bytes.Length.Should().BeLessThan(size, "Dianoga should squeeze JPEG image");
		}

		[SkippableTheory]
		[InlineData("/-/media/Project/Dianoga/Test/svg/svg00.png", 7489)]
		[InlineData("/-/media/Project/Dianoga/Test/svg/svg01.png", 26820)]
		[InlineData("/-/media/Project/Dianoga/Test/svg/svg02.png", 42147)]
		[InlineData("/-/media/Project/Dianoga/Test/svg/svg03.png", 161298)]
		[InlineData("/-/media/Project/Dianoga/Test/svg/svg04.png", 94252)]
		[InlineData("/-/media/Project/Dianoga/Test/svg/svg05.png", 48000)]
		public void SvgSizeTest(string url, int size)
		{
			Skip.IfNot(SvgOptimizationEnabled);
			var wc = new System.Net.WebClient();
			var bytes = wc.DownloadData("https://cd.dockerexamples.localhost/" + url);
			bytes.Length.Should().BeLessThan(size, "Dianoga should squeeze JPEG image");
		}
	}
}
