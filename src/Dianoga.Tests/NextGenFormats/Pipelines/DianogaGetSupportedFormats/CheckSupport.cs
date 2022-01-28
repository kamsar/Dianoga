using System;
using System.Collections.Specialized;
using System.Web;
using Dianoga.NextGenFormats;
using Dianoga.NextGenFormats.Pipelines.DianogaGetSupportedFormats;
using Moq;
using FluentAssertions;
using Xunit;

namespace Dianoga.Tests.NextGenFormats
{
	public class CheckSupportTests
	{
		[Fact]
		public void ShouldFindFormatSupportInAccepts_WhenItIsPresent()
		{
			//Arrange
			var args = new SupportedFormatsArgs()
			{
				Input = "image/avif,image/webp,image/apng,image/*,*/*;q=0.8",
				Prefix = "image/"
			};

			var CheckSupport = new CheckSupport()
			{
				Extension = "webp"
			};

			//Act
			CheckSupport.Process(args);

			//Assert
			args.Extensions.Should().HaveCount(1);
			args.Extensions.Should().Contain("webp");
		}

		[Fact]
		public void ShouldNotFindFormatSupportInAccepts_WhenItIsAbsent()
		{
			//Arrange
			var args = new SupportedFormatsArgs()
			{
				Input = "image/avif,image/webp,image/apng,image/*,*/*;q=0.8",
				Prefix = "image/"
			};

			var CheckSupport = new CheckSupport()
			{
				Extension = "jxl"
			};

			//Act
			CheckSupport.Process(args);

			//Assert
			args.Extensions.Should().HaveCount(0);
		}
	}
}
