using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Dianoga.NextGenFormats;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dianoga.Tests.NextGenFormats
{
	public class HelpersTests
	{
		[Fact]
		public void GetSupportedFormats_ShouldCallAndReturnValueFromdianogaGetSupportedFormatsPipeline()
		{
			//Arrange
			var context = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			var acceptTypes = new[] {"image/webp"};
			request.SetupGet(r => r.AcceptTypes).Returns(acceptTypes);
			context.Setup(ctx => ctx.Request).Returns(request.Object);
			var helpers = new Helpers();
			var pipelineHelpers = new Mock<PipelineHelpers>();
			pipelineHelpers.Setup(h => h.RunDianogaGetSupportedFormatsPipeline(It.IsAny<string[]>())).Returns("webp").Verifiable();
			helpers.PipelineHelpers = pipelineHelpers.Object;

			//Act
			var result = helpers.GetSupportedFormats(context.Object);

			//Assert
			result.Should().Be("webp", "");
			pipelineHelpers.Verify(m=>m.RunDianogaGetSupportedFormatsPipeline(acceptTypes), Times.Once());
		}

		[Fact]
		public void GetSupportedFormats_ShouldNotCallDianogaGetSupportedFormatsPipeline_WhenNoAcceptTypes()
		{
			//Arrange
			var context = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			context.Setup(ctx => ctx.Request).Returns(request.Object);
			var helpers = new Helpers();
			var pipelineHelpers = new Mock<PipelineHelpers>();
			helpers.PipelineHelpers = pipelineHelpers.Object;

			//Act
			var result = helpers.GetSupportedFormats(context.Object);

			//Assert
			result.Should().Be(String.Empty, "");
			pipelineHelpers.Verify(m => m.RunDianogaGetSupportedFormatsPipeline(It.IsAny<string[]>()), Times.Never);
		}

		[Fact]
		public void GetSupportedFormats_ShouldNotCallDianogaGetSupportedFormatsPipeline_WhenNoCustomHeaderValue()
		{
			//Arrange
			var context = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			context.Setup(ctx => ctx.Request).Returns(request.Object);
			var headers = new NameValueCollection { { "customAccept", "" } };
			request.SetupGet(x => x.Headers).Returns(headers);
			var helpers = new Helpers();
			var pipelineHelpers = new Mock<PipelineHelpers>();
			helpers.PipelineHelpers = pipelineHelpers.Object;

			//Act
			var result = helpers.GetSupportedFormats(context.Object);

			//Assert
			result.Should().Be(String.Empty, "");
			pipelineHelpers.Verify(m => m.RunDianogaGetSupportedFormatsPipeline(It.IsAny<string[]>()), Times.Never);
		}

		[Fact]
		public void GetSupportedFormats_ShouldNotCallDianogaGetSupportedFormatsPipeline_WhenNoCustomHeader()
		{
			//Arrange
			var context = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			context.Setup(ctx => ctx.Request).Returns(request.Object);
			var headers = new NameValueCollection { };
			request.SetupGet(x => x.Headers).Returns(headers);
			var helpers = new Helpers();
			var pipelineHelpers = new Mock<PipelineHelpers>();
			helpers.PipelineHelpers = pipelineHelpers.Object;

			//Act
			var result = helpers.GetSupportedFormats(context.Object);

			//Assert
			result.Should().Be(String.Empty, "");
			pipelineHelpers.Verify(m => m.RunDianogaGetSupportedFormatsPipeline(It.IsAny<string[]>()), Times.Never);
		}

		[Fact]
		public void GetSupportedFormats_ShouldCallAndReturnValueFromdianogaGetSupportedFormatsPipeline_WhenNoAcceptTypesAndCustomHeaderIsPresent()
		{
			//Arrange
			var context = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			context.Setup(ctx => ctx.Request).Returns(request.Object);			
			var headers = new NameValueCollection { { "customAccept", "image/webp" } };
			request.SetupGet(x => x.Headers).Returns(headers);
			var queryString = new NameValueCollection();
			queryString.Add("extension", "webp");
			request.SetupGet(r => r.QueryString).Returns(queryString);
			var helpers = new Helpers();
			var pipelineHelpers = new Mock<PipelineHelpers>();
			helpers.PipelineHelpers = pipelineHelpers.Object;

			//Act
			var result = helpers.GetCustomOptions(context.Object);

			//Assert
			result.Should().Be("webp", "");
			pipelineHelpers.Verify(m => m.RunDianogaGetSupportedFormatsPipeline(It.IsAny<string[]>()), Times.Never);
		}

		[Fact]
		public void GetCustomOptions_ShouldTakeValueFromQueryString_WhenExtensionQueryStringIsPresent()
		{
			//Arrange
			var context = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			context.Setup(ctx => ctx.Request).Returns(request.Object);
			var acceptTypes = new[] { "image/webp" };
			request.SetupGet(r => r.AcceptTypes).Returns(acceptTypes);
			var queryString = new NameValueCollection();
			queryString.Add("extension", "webp,avif");
			request.SetupGet(r => r.QueryString).Returns(queryString);
			var helpers = new Helpers();
			var pipelineHelpers = new Mock<PipelineHelpers>();
			helpers.PipelineHelpers = pipelineHelpers.Object;

			//Act
			var result = helpers.GetCustomOptions(context.Object);

			//Assert
			result.Should().Be("webp,avif", "");
			pipelineHelpers.Verify(m => m.RunDianogaGetSupportedFormatsPipeline(It.IsAny<string[]>()), Times.Never);
		}

		[Fact]
		public void GetCustomOptions_ShouldTakeValueFromFromAcceptTypes_WhenExtensionQueryStringIsEmpty()
		{
			//Arrange
			var context = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			context.Setup(ctx => ctx.Request).Returns(request.Object);
			var acceptTypes = new[] { "image/webp", "image/avif" };
			request.SetupGet(r => r.AcceptTypes).Returns(acceptTypes);
			var queryString = new NameValueCollection();
			queryString.Add("extension", "");
			request.SetupGet(r => r.QueryString).Returns(queryString);
			var helpers = new Helpers();
			var pipelineHelpers = new Mock<PipelineHelpers>();
			pipelineHelpers.Setup(h => h.RunDianogaGetSupportedFormatsPipeline(It.IsAny<string[]>())).Returns("webp,avif").Verifiable();
			helpers.PipelineHelpers = pipelineHelpers.Object;

			//Act
			var result = helpers.GetCustomOptions(context.Object);

			//Assert
			result.Should().Be("webp,avif", "");
			pipelineHelpers.Verify(m => m.RunDianogaGetSupportedFormatsPipeline(It.IsAny<string[]>()), Times.Once);
		}

		[Fact]
		public void GetCustomOptions_ShouldTakeValueFromFromCustomHeader_WhenExtensionQueryStringIsEmptyAndAcceptTypeIsEmpty()
		{
			//Arrange
			var context = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			context.Setup(ctx => ctx.Request).Returns(request.Object);
			var headers = new NameValueCollection { { "customAccept", "image/webp" } };
			request.SetupGet(x => x.Headers).Returns(headers);
			var queryString = new NameValueCollection();
			queryString.Add("extension", "");
			request.SetupGet(r => r.QueryString).Returns(queryString);
			var helpers = new Helpers();
			var pipelineHelpers = new Mock<PipelineHelpers>();
			pipelineHelpers.Setup(h => h.RunDianogaGetSupportedFormatsPipeline(It.IsAny<string[]>())).Returns("webp").Verifiable();
			helpers.PipelineHelpers = pipelineHelpers.Object;

			//Act
			var result = helpers.GetCustomOptions(context.Object);

			//Assert
			result.Should().Be("webp", "");
			pipelineHelpers.Verify(m => m.RunDianogaGetSupportedFormatsPipeline(It.IsAny<string[]>()), Times.Once);
		}
	}
}
