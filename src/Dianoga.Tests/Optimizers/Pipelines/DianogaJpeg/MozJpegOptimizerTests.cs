using System.Diagnostics;
using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaJpeg;
using FluentAssertions;
using FluentAssertions.Common;
using Xunit;
using Xunit.Abstractions;

namespace Dianoga.Tests.Optimizers.Pipelines.DianogaJpeg
{
	public class MozJpegOptimizerTests
	{
		ITestOutputHelper output;
		public MozJpegOptimizerTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void ShouldSquishSmallJpegLossless()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\mozjpeg_3.3.1_x86\jpegtran.exe",
				"-progressive", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLargeJpegLossless()
		{
			Test(@"TestImages\large.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\mozjpeg_3.3.1_x86\jpegtran.exe",
				"-progressive", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishSmallJpegLossy()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\mozjpeg_3.3.1_x86\cjpeg.exe",
				"-quality 80", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLargeJpegLossy()
		{
			Test(@"TestImages\large.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\mozjpeg_3.3.1_x86\cjpeg.exe",
				"-quality 80", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldNotSquishCorruptedJpegLossless()
		{
			Test(@"TestImages\corrupted.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\mozjpeg_3.3.1_x86\jpegtran.exe",
				"-quality 80", out var args, out var startingSize);
			args.Stream.Length.Should().IsSameOrEqualTo(startingSize);
			args.IsOptimized.Should().BeFalse();
		}

		[Fact]
		public void ShouldNotSquishCorruptedJpegLossy()
		{
			Test(@"TestImages\corrupted.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\mozjpeg_3.3.1_x86\cjpeg.exe",
				"-quality 80", out var args, out var startingSize);
			args.Stream.Length.Should().IsSameOrEqualTo(startingSize);
			args.IsOptimized.Should().BeFalse();
		}

		private void Test(string imagePath, string exePath, string exeArgs, out OptimizerArgs argsOut, out long startingSize)
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(imagePath))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new MozJpegOptimizer();
			sut.ExePath = exePath;
			sut.AdditionalToolArguments = exeArgs;

			var args = new OptimizerArgs(inputStream);

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
