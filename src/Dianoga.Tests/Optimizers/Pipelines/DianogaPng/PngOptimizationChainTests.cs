using System.IO;
using Dianoga.Optimizers;
using Dianoga.Optimizers.Pipelines.DianogaPng;
using FluentAssertions;
using Xunit;

namespace Dianoga.Tests.Optimizers.Pipelines.DianogaPng
{
	public class PngOptimizationChainTests
	{
		[Fact]
		public void ShouldSquishTestPng()
		{
			var inputStream = new MemoryStream();

			using (var testPng = File.OpenRead(@"Optimizers\Pipelines\DianogaPng\test.png"))
			{
				testPng.CopyTo(inputStream);
			}

			var sut = new PngQuantOptimizer();

			var sut2 = new PngOptimizer();
			sut2.DllPath = @"..\..\..\Dianoga\Dianoga Tools\PNGOptimizer\PNGOptimizerDll.dll";

			var args = new OptimizerArgs(inputStream);

			var startingSize = args.Stream.Length;

			sut.Process(args);
			sut2.Process(args);

			args.Stream.Length.Should().BeLessThan(startingSize).And.BeGreaterThan(0);
			args.IsOptimized.Should().BeTrue();
		}
	}
}
