using System.Diagnostics;
using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaWebP;
using FluentAssertions;
using FluentAssertions.Common;
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
			Test(@"TestImages\small.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\libwebp\cwebp.exe",
				"-q 100 -m 6 -lossless", out var args, out var startingSize);
			args.Stream.Length.Should().Be(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeFalse();
		}

		[Fact]
		public void ShouldSquishLosslessSmallPng()
		{
			Test(@"TestImages\small.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\libwebp\cwebp.exe",
				"-q 100 -m 6 -lossless", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLosslessLargePng()
		{
			Test(@"TestImages\large.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\libwebp\cwebp.exe",
				"-q 100 -m 6 -lossless", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyTestJpeg()
		{
			Test(@"TestImages\large.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\libwebp\cwebp.exe",
				"-q 90 -m 6", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossySmallJpegDefaults()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\libwebp\cwebp.exe",
				"-preset photo -q 80", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyLargeJpegDefaults()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\libwebp\cwebp.exe",
				"-preset photo -q 80", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldNotSquishCorruptedJpegLossy()
		{
			Test(@"TestImages\corrupted.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\libwebp\cwebp.exe",
				"-preset photo -q 80", out var args, out var startingSize);
			args.Stream.Length.Should().IsSameOrEqualTo(startingSize);
			args.IsOptimized.Should().BeFalse();
		}

		[Fact]
		public void ShouldSquishLossySmallPngHighAlpha()
		{
			Test(@"TestImages\small.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\libwebp\cwebp.exe",
				"-q 90 -alpha_q 100 -m 6", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossySmallPngDefaults()
		{
			Test(@"TestImages\small.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\libwebp\cwebp.exe",
				"-preset icon", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyLargePngDefaults()
		{
			Test(@"TestImages\large.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\libwebp\cwebp.exe",
				"-preset icon", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyTestGif()
		{
			Test(@"TestImages\small.gif",
				@"..\..\..\..\Dianoga\Dianoga Tools\libwebp\gif2webp.exe",
				"-q 90 -lossy", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLosslessTestGif()
		{
			Test(@"TestImages\small.gif",
				@"..\..\..\..\Dianoga\Dianoga Tools\libwebp\gif2webp.exe",
				"-q 80", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		private void Test(string imagePath, string exePath, string exeArgs, out OptimizerArgs argsOut, out long startingSize)
		{
			var inputStream = new MemoryStream();

			using (var inputFileStream = File.OpenRead(imagePath))
			{
				inputFileStream.CopyTo(inputStream);
			}

			var sut = new WebPOptimizer
			{
				ExePath = exePath, 
				AdditionalToolArguments = exeArgs
			};

			var opts = new Sitecore.Resources.Media.MediaOptions();
			opts.CustomOptions["extension"] = "webp";
			var args = new OptimizerArgs(inputStream, opts);

			startingSize = args.Stream.Length;

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			sut.Process(args);
			stopwatch.Stop();
			output.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}ms");

			argsOut = args;
		}
	}
}
