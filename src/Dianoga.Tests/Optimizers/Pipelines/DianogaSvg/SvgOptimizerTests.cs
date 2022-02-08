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
				@"..\..\..\..\Dianoga\Dianoga Tools\SVGO\svgo-win.exe",
				"-", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishSmallSvgWithArgs()
		{
			Test(@"TestImages\small.svg",
				@"..\..\..\..\Dianoga\Dianoga Tools\SVGO\svgo-win.exe",
				"-", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldFollowConfigurationWithArgs()
		{
			//Pass configuration that disable removeViewBox plugin
			Test(@"TestImages\small.svg",
				@"..\..\..\..\Dianoga\Dianoga Tools\SVGO\svgo-win.exe",
				"- --config svgo.sample.config.js", out var args1, out var startingSize1);
			var file1 = new StreamReader(args1.Stream).ReadToEnd();

			file1.Should().Contain("viewBox");

			//Do not pass any configuration
			Test(@"TestImages\small.svg",
				@"..\..\..\..\Dianoga\Dianoga Tools\SVGO\svgo-win.exe",
				"-", out var args2, out var startingSize2);
			var file2 = new StreamReader(args2.Stream).ReadToEnd();

			file2.Should().NotContain("viewBox");
		}

		[Fact]
		public void ShouldSquishLargeSvg()
		{
			Test(@"TestImages\large.svg",
				@"..\..\..\..\Dianoga\Dianoga Tools\SVGO\svgo-win.exe",
				"-", out var args, out var startingSize);
			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldNotSquishCorruptedJpeg()
		{
			Test(@"TestImages\corrupted.jpg",
				@"..\..\..\..\Dianoga\Dianoga Tools\SVGO\svgo-win.exe",
				"-", out var args, out var startingSize);
			args.Stream.Length.Should().IsSameOrEqualTo(startingSize);
			args.IsOptimized.Should().BeFalse();
		}

		private void Test(string imagePath, string exePath, string exeArgs, out OptimizerArgs argsOut, out long startingSize)
		{
			var inputStream = new MemoryStream();

			using (var testFile = File.OpenRead(imagePath))
			{
				testFile.CopyTo(inputStream);
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
