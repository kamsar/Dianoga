using System.Diagnostics;
using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaSvg;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Dianoga.Tests.Optimizers.Pipelines.DianogaSvg
{
	public class SvgOptimizerTests
	{
		ITestOutputHelper output;
		public SvgOptimizerTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void ShouldSquishSmallSvg()
		{
			Test(@"TestImages\small.svg",
				@"../../Optimizers/Pipelines/DianogaSvg/SVGO/node.exe",
				"--disable=removeUselessDefs --disable=cleanupIDs");
		}

		[Fact]
		public void ShouldSquishLargeSvg()
		{
			Test(@"TestImages\large.svg",
				@"../../Optimizers/Pipelines/DianogaSvg/SVGO/node.exe",
				"--disable=removeUselessDefs --disable=cleanupIDs");
		}

		private void Test(string imagePath, string exePath, string exeArgs)
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(imagePath))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new SvgoOptimizer();
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
