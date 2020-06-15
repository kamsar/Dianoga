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
	public class JpegOptimOptimizerTests
	{
		ITestOutputHelper output;
		public JpegOptimOptimizerTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void ShouldSquishSmallJpeg()
		{
			Test(@"TestImages\small.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\jpegoptim-windows\jpegoptim.exe",
				"--strip-all --all-progressive -m90", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLargeJpeg()
		{
			Test(@"TestImages\large.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\jpegoptim-windows\jpegoptim.exe",
				"--strip-all --all-progressive -m90", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldNotSquishCorruptedJpegLossy()
		{
			Test(@"TestImages\corrupted.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\jpegoptim-windows\jpegoptim.exe",
				"--strip-all --all-progressive -m90", out var args, out var startingSize);
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

			var sut = new JpegOptimOptimizer();
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
