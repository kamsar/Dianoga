using System.Diagnostics;
using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaPng;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Dianoga.Tests.Optimizers.Pipelines.DianogaPng
{
	public class PngOptimizerTests
	{
		ITestOutputHelper output;
		public PngOptimizerTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void ShouldSquishSmallPng()
		{
			Test(@"TestImages\small.png",
				@"..\..\..\Dianoga\Dianoga Tools\PNGOptimizer\PNGOptimizerCL.exe",
				"");
		}

		[Fact]
		public void ShouldSquishLargePng()
		{
			Test(@"TestImages\large.png",
				@"..\..\..\Dianoga\Dianoga Tools\PNGOptimizer\PNGOptimizerCL.exe",
				"");
		}

		private void Test(string imagePath, string exePath, string exeArgs)
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(imagePath))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new PngOptimizer();
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
