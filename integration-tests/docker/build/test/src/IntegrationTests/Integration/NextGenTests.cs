using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using Endjin.Retry.Async;
using FluentAssertions;
using Xunit;

namespace Integration
{
	public class NextGenTests : BaseTests
	{
		/* JPEG XL shows very bad results on PNG images
		Test fails, it is not recomended to turn it on for PNG on 2022/02/12
		[SkippableTheory]
		[InlineData("/-/media/Project/Dianoga/Test/png/png00.png", 100825)]
		[InlineData("/-/media/Project/Dianoga/Test/png/png01.png", 277679)]
		//[InlineData("/-/media/Project/Dianoga/Test/png/png02.png", 250645)]
		//[InlineData("/-/media/Project/Dianoga/Test/png/png03.png", 110259)]
		//[InlineData("/-/media/Project/Dianoga/Test/png/png04.png", 114270)]
		//[InlineData("/-/media/Project/Dianoga/Test/png/png05.png", 56132)]
		public void PngJxlTest(string url, int size)
		{
			Skip.IfNot(JxlOptimizationEnabled);
			SizeTest(url, size, "Dianoga should squeeze PNG image", "image/jxl", "ftypjxl", 20);
		}*/

		[SkippableTheory]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg00.jpg", 4427080)]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg01.jpg", 4822280)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg02.jpg", 2261738)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg03.jpg", 5684535)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg04.jpg", 3274103)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg05.jpg", 5652302)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg06.jpg", 5220868)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg07.jpg", 5220868)]
		public void JpegJxlTest(string url, int size)
		{
			Skip.IfNot(JxlOptimizationEnabled);
			SizeTest(url, size, "Dianoga should squeeze JPEG image", "image/jxl", "ftypjxl", 30);
		}


		[SkippableTheory]
		[InlineData("/-/media/Project/Dianoga/Test/png/png00.png", 100825)]
		[InlineData("/-/media/Project/Dianoga/Test/png/png01.png", 277679)]
		//[InlineData("/-/media/Project/Dianoga/Test/png/png02.png", 250645)]
		//[InlineData("/-/media/Project/Dianoga/Test/png/png03.png", 110259)]
		//[InlineData("/-/media/Project/Dianoga/Test/png/png04.png", 114270)]
		//[InlineData("/-/media/Project/Dianoga/Test/png/png05.png", 56132)]
		public void PngAvifTest(string url, int size)
		{
			Skip.IfNot(AvifOptimizationEnabled);
			SizeTest(url, size, "Dianoga should squeeze PNG image", "image/avif", "avif");
		}

		[SkippableTheory]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg00.jpg", 4427080)]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg01.jpg", 4822280)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg02.jpg", 2261738)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg03.jpg", 5684535)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg04.jpg", 3274103)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg05.jpg", 5652302)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg06.jpg", 5220868)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg07.jpg", 1403617)]
		public void JpegAvifTest(string url, int size)
		{
			Skip.IfNot(AvifOptimizationEnabled);
			SizeTest(url, size, "Dianoga should squeeze JPEG image", "image/avif", "avif");
		}

		[SkippableTheory]
		[InlineData("/-/media/Project/Dianoga/Test/png/png00.png", 100825)]
		[InlineData("/-/media/Project/Dianoga/Test/png/png01.png", 277679)]
		//[InlineData("/-/media/Project/Dianoga/Test/png/png02.png", 250645)]
		//[InlineData("/-/media/Project/Dianoga/Test/png/png03.png", 110259)]
		//[InlineData("/-/media/Project/Dianoga/Test/png/png04.png", 114270)]
		//[InlineData("/-/media/Project/Dianoga/Test/png/png05.png", 56132)]
		public void PngWebpTest(string url, int size)
		{
			//Skip.IfNot(WebpOptimizationEnabled);
			SizeTest(url, size, "Dianoga should squeeze PNG image", "image/webp", "WEBP");
		}

		[SkippableTheory]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg00.jpg", 4427080)]
		[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg01.jpg", 4822280)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg02.jpg", 2261738)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg03.jpg", 5684535)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg04.jpg", 3274103)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg05.jpg", 5652302)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg06.jpg", 5220868)]
		//[InlineData("/-/media/Project/Dianoga/Test/jpeg/jpg07.jpg", 1403617)]
		public void JpegWebpTest(string url, int size)
		{
			Skip.IfNot(WebpOptimizationEnabled);
			SizeTest(url, size, "Dianoga should squeeze JPEG image", "image/webp", "WEBP");
		}
		public void SizeTest(string url, int size, string message, string accept, string content, int conversionTimeout = 20)
		{
			var rand = new Random(DateTime.Now.Millisecond);
			var seed = $"?seed={rand.Next(Int32.MaxValue)}";
			var sleepService = new SleepService();
			if (Sync)
			{
				var request = WebRequest.Create(CDHostname + url + seed);
				request.Headers = GetHeaders(accept);
				request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
				var response = request.GetResponse();
				var string1 = ResponseToString(response);
				var length = response.ContentLength;
				length.Should().BeLessThan(size, message);
				string1.IndexOf(content).Should().BeGreaterThan(-1);
			}
			else
			{
				var request = WebRequest.Create(CDHostname + url + seed);
				request.Headers = GetHeaders(accept);
				request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
				var response = request.GetResponse();
				var string1 = ResponseToString(response);
				var initialSize = response.ContentLength;
				string1.IndexOf(content, StringComparison.Ordinal).Should().Be(-1);
				//How to find proper value, how much time file conversion will take?
				sleepService.Sleep(new TimeSpan(0, 0, conversionTimeout));

				request = WebRequest.Create(CDHostname + url + seed);
				request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
				request.Headers = GetHeaders(accept);
				response = request.GetResponse();
				var string2 = ResponseToString(response);
				var squeezeSize = response.ContentLength;
				squeezeSize.Should().BeLessThan(initialSize, message);
				squeezeSize.Should().BeLessThan(size, message);
				string2.IndexOf(content, StringComparison.Ordinal).Should().BeGreaterThan(-1);
			}
		}



		private WebHeaderCollection GetHeaders(string accept)
		{
			var headers = new WebHeaderCollection();
			headers.Add(HttpRequestHeader.Accept, accept);
			return headers;
		}
	}
}
