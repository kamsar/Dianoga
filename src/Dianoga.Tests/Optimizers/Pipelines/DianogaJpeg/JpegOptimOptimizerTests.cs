using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaJpeg;
using FluentAssertions;
using Xunit;

namespace Dianoga.Tests.Optimizers.Pipelines.DianogaJpeg
{
	public class JpegOptimOptimizerTests
	{
		[Fact]
		public void ShouldSquishTestJpeg()
		{
			var inputStream = new MemoryStream();

			using (var testJpeg = File.OpenRead(@"Optimizers\Pipelines\DianogaJpeg\test.jpg"))
			{
				testJpeg.CopyTo(inputStream);
			}

			var sut = new JpegOptimOptimizer();
			sut.ExePath = @"..\..\..\Dianoga\Dianoga Tools\jpegoptim-windows\jpegoptim.exe";
			sut.AdditionalToolArguments = "--strip-all --all-progressive -m90";

			var args = new OptimizerArgs(inputStream);

			var startingSize = args.Stream.Length;

			sut.Process(args);

			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}
	}
}
