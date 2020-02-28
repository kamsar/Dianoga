using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaPng;
using FluentAssertions;
using Xunit;

namespace Dianoga.Tests.Optimizers.Pipelines.DianogaPng
{
	public class PngQuantCliOptimizerTests
	{
		[Fact]
		public void ShouldSquishTestPng()
		{
			var inputStream = new MemoryStream();

			using (var testPng = File.OpenRead(@"Optimizers\Pipelines\DianogaPng\test.png"))
			{
				testPng.CopyTo(inputStream);
			}

			var sut = new PngQuantCliOptimizer();
			sut.ExePath = @"..\..\..\Dianoga\Dianoga Tools\pngquant\pngquant.exe";

			var args = new OptimizerArgs(inputStream);

			var startingSize = args.Stream.Length;

			sut.Process(args);

			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}
	}
}
