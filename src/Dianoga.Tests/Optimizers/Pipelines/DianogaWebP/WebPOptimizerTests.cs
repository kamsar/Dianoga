using System.Diagnostics;
using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaWebP;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Dianoga.Tests.Optimizers.Pipelines.DianogaWebP
{
	public class WebPOptimizerTests
	{
		ITestOutputHelper output;
		public WebPOptimizerTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void ShouldReturnOriginalStreamWhenOptimizedImageSizeIsGreater()
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(@"TestImages\small.jpg"))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new WebPOptimizer();
			sut.ExePath = @"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe";
			sut.AdditionalToolArguments = "-q 100 -m 6 -lossless";

			var opts = new Sitecore.Resources.Media.MediaOptions();
			opts.CustomOptions["extension"] = "webp";
			var args = new OptimizerArgs(inputStream, opts);

			var startingSize = args.Stream.Length;

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			sut.Process(args);
			stopwatch.Stop();
			output.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}ms");

			args.Stream.Length.Should().Be(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeFalse();
		}

		[Fact]
		public void ShouldSquishLosslessSmallPng()
		{
			Test(@"TestImages\small.png",
				@"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe",
				"-q 100 -m 6 -lossless");
		}

		[Fact]
		public void ShouldSquishLosslessLargePng()
		{
			Test(@"TestImages\large.png",
				@"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe",
				"-q 100 -m 6 -lossless");
		}

		[Fact]
		public void ShouldSquishLossyTestJpeg()
		{
			Test(@"TestImages\large.jpg",
				@"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe",
				"-q 90 -m 6");
		}

		[Fact]
		public void ShouldSquishLossySmallJpegDefaults()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe",
				"-preset photo -q 80");
		}

		[Fact]
		public void ShouldSquishLossyLargeJpegDefaults()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe",
				"-preset photo -q 80");
		}

		[Fact]
		public void ShouldSquishLossySmallPngHighAlpha()
		{
			Test(@"TestImages\small.png",
				@"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe",
				"-q 90 -alpha_q 100 -m 6");
		}

		[Fact]
		public void ShouldSquishLossySmallPngDefaults()
		{
			Test(@"TestImages\small.png",
				@"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe",
				"-preset icon");
		}

		[Fact]
		public void ShouldSquishLossyLargePngDefaults()
		{
			Test(@"TestImages\large.png",
				@"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe",
				"-preset icon");
		}

		[Fact]
		public void ShouldSquishLossyTestGif()
		{
			Test(@"TestImages\small.gif",
				@"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\gif2webp.exe",
				"-q 90 -lossy");
		}

		[Fact]
		public void ShouldSquishLosslessTestGif()
		{
			Test(@"TestImages\small.gif",
				@"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\gif2webp.exe",
				"-q 80");
		}

		private void Test(string imagePath, string exePath, string exeArgs)
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(imagePath))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new WebPOptimizer();
			sut.ExePath = exePath;
			sut.AdditionalToolArguments = exeArgs;

			var opts = new Sitecore.Resources.Media.MediaOptions();
			opts.CustomOptions["extension"] = "webp";
			var args = new OptimizerArgs(inputStream, opts);

			var startingSize = args.Stream.Length;

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			sut.Process(args);
			stopwatch.Stop();
			output.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}ms");

			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}
	}
}
