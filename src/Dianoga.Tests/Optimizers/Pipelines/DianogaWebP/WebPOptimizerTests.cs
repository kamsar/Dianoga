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
		public void ShouldSquishTestJpeg()
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(@"Optimizers\Pipelines\DianogaWebP\test.jpg"))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new WebPOptimizer();
			sut.ExePath = @"..\..\..\Dianoga\Dianoga Tools\libwebp-0.4.1-windows-x64\bin\cwebp.exe";

			var args = new OptimizerArgs(inputStream);

			var startingSize = args.Stream.Length;

			sut.Process(args);

			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}

		[Fact]
		public void ShouldSquishTestPng()
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(@"Optimizers\Pipelines\DianogaWebP\test.png"))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new WebPOptimizer();
			sut.ExePath = @"..\..\..\Dianoga\Dianoga Tools\libwebp-0.4.1-windows-x64\bin\cwebp.exe";

			var args = new OptimizerArgs(inputStream);

			var startingSize = args.Stream.Length;

			sut.Process(args);

			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}
	}
}
