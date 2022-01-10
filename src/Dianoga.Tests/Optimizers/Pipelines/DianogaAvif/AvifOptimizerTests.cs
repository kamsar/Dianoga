using System.Diagnostics;
using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaAvif;
using FluentAssertions;
using FluentAssertions.Common;
using Xunit;
using Xunit.Abstractions;

namespace Dianoga.Tests.Optimizers.Pipelines.DianogaAvif
{
	public class AvifOptimizerTests
	{
		ITestOutputHelper output;
		public AvifOptimizerTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void ShouldReturnOriginalStreamWhenOptimizedImageSizeIsGreater()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\avif\avifenc.exe",
				"-s 10 -l", out var args, out var startingSize);
			args.Stream.Length.Should().Be(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeFalse();
		}

		[Fact]
		public void ShouldSquishLosslessSmallPng()
		{
			Test(@"TestImages\small.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\avif\avifenc.exe",
				"-s 10 -l", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLosslessLargePngButBeTooBig()
		{
			Test(@"TestImages\large.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\avif\avifenc.exe",
				"-s 10 -l", out var args, out var startingSize);
			args.Stream.Length.Should().Be(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeFalse();
		}

		[Fact]
		public void ShouldSquishLossySmallJpegDefaults()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\avif\avifenc.exe",
				"-s 10", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyLargeJpegDefaults()
		{
			Test(@"TestImages\large.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\avif\avifenc.exe",
				"-s 10", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldNotSquishCorruptedJpegLossy()
		{
			Test(@"TestImages\corrupted.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\avif\avifenc.exe",
				"-s 10", out var args, out var startingSize);
			args.Stream.Length.Should().IsSameOrEqualTo(startingSize);
			args.IsOptimized.Should().BeFalse();
		}

		[Fact]
		public void ShouldSquishLossySmallPngDefaults()
		{
			Test(@"TestImages\small.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\avif\avifenc.exe",
				"-s 10", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyLargePngDefaults()
		{
			Test(@"TestImages\large.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\avif\avifenc.exe",
				"-s 10", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		private void Test(string imagePath, string exePath, string exeArgs, out OptimizerArgs argsOut, out long startingSize)
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(imagePath))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new AvifOptimizer();
			sut.ExePath = exePath;
			sut.AdditionalToolArguments = exeArgs;

			var opts = new Sitecore.Resources.Media.MediaOptions();
			opts.CustomOptions["extension"] = "avif";
			var args = new OptimizerArgs(inputStream, opts, imagePath);

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
