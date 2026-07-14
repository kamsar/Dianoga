using System;
using System.Collections.Specialized;
using System.Web;
using System.Xml;
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

		[Fact]
		public void ShouldNotFindFormatSupport_WhenUserAgentMatchesUnsupportedUserAgentRule()
		{
			//Arrange
			var args = new SupportedFormatsArgs()
			{
				Input = "image/avif,image/webp,image/apng,image/*,*/*;q=0.8",
				Prefix = "image/"
			};

			var checkSupport = new TestableCheckSupport("Mozilla/4.0 (compatible; ms-office; MSOffice 16)")
			{
				Extension = "webp"
			};
			checkSupport.UnsupportedUserAgents.Add(new UserAgentRule { Value = "ms-office", MatchType = "Contains" });

			//Act
			checkSupport.Process(args);

			//Assert
			args.Extensions.Should().HaveCount(0);
		}

		[Fact]
		public void ShouldFindFormatSupport_WhenUserAgentDoesNotMatchUnsupportedUserAgentRules()
		{
			//Arrange
			var args = new SupportedFormatsArgs()
			{
				Input = "image/avif,image/webp,image/apng,image/*,*/*;q=0.8",
				Prefix = "image/"
			};

			var checkSupport = new TestableCheckSupport("Mozilla/5.0 (Windows NT 10.0) Chrome/120.0")
			{
				Extension = "webp"
			};
			checkSupport.UnsupportedUserAgents.Add(new UserAgentRule { Value = "ms-office", MatchType = "Contains" });
			checkSupport.UnsupportedUserAgents.Add(new UserAgentRule { Value = "Some Legacy Agent", MatchType = "ExactMatch" });

			//Act
			checkSupport.Process(args);

			//Assert
			args.Extensions.Should().HaveCount(1);
			args.Extensions.Should().Contain("webp");
		}

		[Fact]
		public void ShouldFindFormatSupport_WhenUserAgentIsNullAndRulesAreConfigured()
		{
			//Arrange
			var args = new SupportedFormatsArgs()
			{
				Input = "image/avif,image/webp,image/apng,image/*,*/*;q=0.8",
				Prefix = "image/"
			};

			var checkSupport = new TestableCheckSupport(null)
			{
				Extension = "webp"
			};
			checkSupport.UnsupportedUserAgents.Add(new UserAgentRule { Value = "ms-office", MatchType = "Contains" });

			//Act
			checkSupport.Process(args);

			//Assert
			args.Extensions.Should().Contain("webp");
		}

		[Fact]
		public void ShouldFindFormatSupport_WhenUnsupportedUserAgentsIsNull()
		{
			//Arrange
			var args = new SupportedFormatsArgs()
			{
				Input = "image/avif,image/webp,image/apng,image/*,*/*;q=0.8",
				Prefix = "image/"
			};

			var checkSupport = new TestableCheckSupport("Mozilla/4.0 (compatible; ms-office)")
			{
				Extension = "webp",
				UnsupportedUserAgents = null
			};

			//Act
			checkSupport.Process(args);

			//Assert
			args.Extensions.Should().Contain("webp");
		}

		[Fact]
		public void AddUnsupportedUserAgent_ShouldMapValueAndMatchTypeFromConfigNode()
		{
			//Arrange
			var checkSupport = new CheckSupport();

			//Act
			checkSupport.AddUnsupportedUserAgent(CreateConfigNode("<userAgent value=\"ms-office\" matchType=\"ExactMatch\" />"));

			//Assert
			checkSupport.UnsupportedUserAgents.Should().HaveCount(1);
			checkSupport.UnsupportedUserAgents[0].Value.Should().Be("ms-office");
			checkSupport.UnsupportedUserAgents[0].MatchType.Should().Be("ExactMatch");
		}

		[Fact]
		public void AddUnsupportedUserAgent_ShouldDefaultToContains_WhenMatchTypeIsMissing()
		{
			//Arrange
			var checkSupport = new CheckSupport();

			//Act
			checkSupport.AddUnsupportedUserAgent(CreateConfigNode("<userAgent value=\"ms-office\" />"));

			//Assert
			checkSupport.UnsupportedUserAgents.Should().HaveCount(1);
			checkSupport.UnsupportedUserAgents[0].MatchType.Should().Be(UserAgentRule.MatchTypeContains);
		}

		[Theory]
		[InlineData("<userAgent matchType=\"Contains\" />")]
		[InlineData("<userAgent value=\"\" matchType=\"Contains\" />")]
		public void AddUnsupportedUserAgent_ShouldSkipRule_WhenValueIsMissingOrEmpty(string xml)
		{
			//Arrange
			var checkSupport = new CheckSupport();

			//Act
			checkSupport.AddUnsupportedUserAgent(CreateConfigNode(xml));

			//Assert
			checkSupport.UnsupportedUserAgents.Should().BeEmpty();
		}

		private static XmlNode CreateConfigNode(string xml)
		{
			var document = new XmlDocument();
			document.LoadXml(xml);
			return document.DocumentElement;
		}

		private class TestableCheckSupport : CheckSupport
		{
			private readonly string _userAgent;

			public TestableCheckSupport(string userAgent)
			{
				_userAgent = userAgent;
			}

			protected override string GetUserAgent() => _userAgent;
		}
	}
}
