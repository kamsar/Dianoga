using Dianoga.NextGenFormats;
using FluentAssertions;
using Xunit;

namespace Dianoga.Tests.NextGenFormats
{
	public class UserAgentRuleTests
	{
		[Theory]
		[InlineData("Mozilla/4.0 (compatible; ms-office; MSOffice 16)", "ms-office", "Contains", true)]
		[InlineData("Mozilla/4.0 (compatible; MS-OFFICE; MSOffice 16)", "ms-office", "Contains", true)]
		[InlineData("Mozilla/5.0 (Windows NT 10.0) Chrome/120.0", "ms-office", "Contains", false)]
		[InlineData("ms-office", "MS-Office", "ExactMatch", true)]
		[InlineData("ms-office extra", "ms-office", "ExactMatch", false)]
		[InlineData("Microsoft Office/16.0", "microsoft office", "StartsWith", true)]
		[InlineData("Microsoft Office/16.0", "microsoft office", "  startswith  ", true)]
		[InlineData("Something Microsoft Office/16.0", "Microsoft Office", "StartsWith", false)]
		public void IsMatch_ShouldRespectMatchTypeCaseInsensitively(string userAgent, string value, string matchType, bool expected)
		{
			var rule = new UserAgentRule { Value = value, MatchType = matchType };

			rule.IsMatch(userAgent).Should().Be(expected);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public void IsMatch_ShouldReturnFalse_WhenUserAgentIsNullOrEmpty(string userAgent)
		{
			var rule = new UserAgentRule { Value = "ms-office", MatchType = "Contains" };

			rule.IsMatch(userAgent).Should().BeFalse();
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public void IsMatch_ShouldReturnFalse_WhenValueIsNullOrEmpty(string value)
		{
			var rule = new UserAgentRule { Value = value, MatchType = "Contains" };

			rule.IsMatch("ms-office").Should().BeFalse();
		}

		[Fact]
		public void IsMatch_ShouldFallBackToContains_WhenMatchTypeIsUnknown()
		{
			var rule = new UserAgentRule { Value = "ms-office", MatchType = "Regex" };

			rule.IsMatch("Mozilla/4.0 (compatible; ms-office)").Should().BeTrue();
			rule.IsMatch("Mozilla/5.0 Chrome/120.0").Should().BeFalse();
		}

		[Fact]
		public void IsMatch_ShouldDefaultToContains_WhenMatchTypeIsMissing()
		{
			var rule = new UserAgentRule { Value = "ms-office", MatchType = null };

			rule.IsMatch("Mozilla/4.0 (compatible; ms-office)").Should().BeTrue();
		}
	}
}
