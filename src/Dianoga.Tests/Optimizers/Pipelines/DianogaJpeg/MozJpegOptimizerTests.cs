using System.Diagnostics;
using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaJpeg;
using FluentAssertions;
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
				"-progressive");
		}

		[Fact]
		public void ShouldSquishLargeJpegLossless()
		{
			Test(@"TestImages\large.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\mozjpeg_3.3.1_x86\jpegtran.exe",
				"-progressive");
		}

		[Fact]
		public void ShouldSquishSmallJpegLossy()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\mozjpeg_3.3.1_x86\cjpeg.exe",
				"-quality 80");
		}

		[Fact]
		public void ShouldSquishLargeJpegLossy()
		{
			Test(@"TestImages\large.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\mozjpeg_3.3.1_x86\cjpeg.exe",
				"-quality 80");
		}

		private void Test(string imagePath, string exePath, string exeArgs)
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
