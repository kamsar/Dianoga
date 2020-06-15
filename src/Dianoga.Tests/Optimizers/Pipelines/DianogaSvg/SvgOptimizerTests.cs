using System.Diagnostics;
using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaSvg;
using FluentAssertions;
using FluentAssertions.Common;
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
				@"..\..\..\..\Dianoga\Dianoga Tools\SVGO\svgop.exe",
				"", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLargeSvg()
		{
			Test(@"TestImages\large.svg",
				@"..\..\..\..\Dianoga\Dianoga Tools\SVGO\svgop.exe",
				"", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldNotSquishCorruptedJpeg()
		{
			Test(@"TestImages\corrupted.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\SVGO\svgop.exe",
				"", out var args, out var startingSize);
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

			var sut = new SvgoOptimizer();
			sut.ExePath = exePath;
			sut.AdditionalToolArguments = exeArgs;

			var args = new OptimizerArgs(inputStream);

			startingSize = args.Stream.Length;

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			sut.Process(args);
			stopwatch.Stop();
			output.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}ms Size: {args.Stream.Length}");

			argsOut = args;
		}
	}
}
