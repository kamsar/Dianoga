using System.Diagnostics;
using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaJxl;
using FluentAssertions;
using FluentAssertions.Common;
using Xunit;
using Xunit.Abstractions;

namespace Dianoga.Tests.Optimizers.Pipelines.DianogaJxl
{
	public class JxlOptimizerTests
	{
		ITestOutputHelper output;
		public JxlOptimizerTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void ShouldReturnOriginalStreamWhenOptimizedImageSizeIsGreater()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\jxl\cjxl.exe",
				"-q 100", out var args, out var startingSize);
			args.Stream.Length.Should().Be(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeFalse();
		}

		[Fact]
		public void ShouldSquishLosslessSmallPng()
		{
			Test(@"TestImages\small.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\jxl\cjxl.exe",
				"-q 100 --num_threads 1", out var args, out var startingSize);

			/*
			num_threads is often required parameter
			JPEG XL encoder v0.6.1 a205468 [SSE4,Scalar]
			Failed to choose default num_threads; you can avoid this error by specifying a --num_threads N argument.
			 */
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLosslessLargePng()
		{
			Test(@"TestImages\large.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\jxl\cjxl.exe",
				"-q 100 --num_threads 1", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyTestJpeg()
		{
			Test(@"TestImages\large.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\jxl\cjxl.exe",
				"-q 90 --num_threads 1", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossySmallJpegDistance()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\jxl\cjxl.exe",
				"-d 1 --num_threads 1", out var args, out var startingSize);

			/*
			 -d maxError, --distance=maxError
		    Max. butteraugli distance, lower = higher quality. Range: 0 .. 25.
		    0.0 = mathematically lossless. Default for already-lossy input (JPEG/GIF).
		    1.0 = visually lossless. Default for other input.
		    Recommended range: 0.5 .. 3.0.
			 */
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyLargeJpegDistance()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\jxl\cjxl.exe",
				"-d 1 --num_threads 1", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldNotSquishCorruptedJpegLossy()
		{
			Test(@"TestImages\corrupted.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\jxl\cjxl.exe",
				"-d 1", out var args, out var startingSize);
			args.Stream.Length.Should().IsSameOrEqualTo(startingSize);
			args.IsOptimized.Should().BeFalse();
		}

		[Fact]
		public void ShouldSquishLossySmallPngHighAlpha()
		{
			Test(@"TestImages\small.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\jxl\cjxl.exe",
				"-q 90 --num_threads 1", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossySmallPngDistance()
		{
			Test(@"TestImages\small.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\jxl\cjxl.exe",
				"-d 1 --num_threads 1", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyLargePngDistance()
		{
			Test(@"TestImages\large.png",
				@"..\..\..\..\Dianoga\Dianoga Tools\jxl\cjxl.exe",
				"-d 1 --num_threads 1", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyTestGif()
		{
			Test(@"TestImages\small.gif",
				@"..\..\..\..\Dianoga\Dianoga Tools\jxl\cjxl.exe",
				"-q 90 --num_threads 1", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLosslessTestGif()
		{
			Test(@"TestImages\small.gif",
				@"..\..\..\..\Dianoga\Dianoga Tools\jxl\cjxl.exe",
				"-q 80 --num_threads 1", out var args, out var startingSize);
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

			var sut = new JxlOptimizer
			{
				ExePath = exePath, 
				AdditionalToolArguments = exeArgs
			};

			var opts = new Sitecore.Resources.Media.MediaOptions();
			opts.CustomOptions["extension"] = "jxl";
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
