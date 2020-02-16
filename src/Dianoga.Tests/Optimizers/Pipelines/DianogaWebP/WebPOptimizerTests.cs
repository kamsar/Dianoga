using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaWebP;
using FluentAssertions;
using Xunit;

namespace Dianoga.Tests.Optimizers.Pipelines.DianogaWebP
{
	public class WebPOptimizerTests
	{
		[Fact]
		public void ShouldReturnOriginalStreamWhenOptimizedImageSizeIsGreater()
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(@"Optimizers\Pipelines\DianogaWebP\test.jpg"))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new WebPOptimizer();
			sut.ExePath = @"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe";
			sut.AdditionalToolArguments = "-q 100 -m 6 -lossless";

			var args = new OptimizerArgs(inputStream);
			args.AcceptWebP = true;

			var startingSize = args.Stream.Length;

			sut.Process(args);

			args.Stream.Length.Should().Be(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeFalse();
		}

		[Fact]
		public void ShouldSquishLosslessTestPng()
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(@"Optimizers\Pipelines\DianogaWebP\test.png"))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new WebPOptimizer();
			sut.ExePath = @"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe";
			sut.AdditionalToolArguments = "-q 100 -m 6 -lossless";

			var args = new OptimizerArgs(inputStream);
			args.AcceptWebP = true;

			var startingSize = args.Stream.Length;

			sut.Process(args);

			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLosslessTestGif()
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(@"Optimizers\Pipelines\DianogaWebP\test.gif"))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new WebPOptimizer();
			sut.ExePath = @"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\gif2webp.exe";
			sut.AdditionalToolArguments = "-q 100 -m 6";

			var args = new OptimizerArgs(inputStream);
			args.AcceptWebP = true;

			var startingSize = args.Stream.Length;

			sut.Process(args);

			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyTestJpeg()
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(@"Optimizers\Pipelines\DianogaWebP\test.jpg"))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new WebPOptimizer();
			sut.ExePath = @"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe";
			sut.AdditionalToolArguments = "-q 90 -m 6";

			var args = new OptimizerArgs(inputStream);
			args.AcceptWebP = true;

			var startingSize = args.Stream.Length;

			sut.Process(args);

			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyTestPng()
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(@"Optimizers\Pipelines\DianogaWebP\test.png"))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new WebPOptimizer();
			sut.ExePath = @"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\cwebp.exe";
			sut.AdditionalToolArguments = "-q 90 -alpha_q 100 -m 6";

			var args = new OptimizerArgs(inputStream);
			args.AcceptWebP = true;

			var startingSize = args.Stream.Length;

			sut.Process(args);

			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishLossyTestGif()
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(@"Optimizers\Pipelines\DianogaWebP\test.gif"))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new WebPOptimizer();
			sut.ExePath = @"..\..\..\Dianoga\Dianoga Tools\libwebp-1.1.0-windows-x64\bin\gif2webp.exe";
			sut.AdditionalToolArguments = "-q 90 -m 6 -lossy";

			var args = new OptimizerArgs(inputStream);
			args.AcceptWebP = true;

			var startingSize = args.Stream.Length;

			sut.Process(args);

			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}
	}
}
