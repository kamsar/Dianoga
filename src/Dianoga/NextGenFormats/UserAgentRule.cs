using System;

namespace Dianoga.NextGenFormats
{
	public class UserAgentRule
	{
		public const string MatchTypeContains = "Contains";
		public const string MatchTypeExactMatch = "ExactMatch";
		public const string MatchTypeStartsWith = "StartsWith";

		public string Value { get; set; }

		public string MatchType { get; set; } = MatchTypeContains;

		public virtual bool IsMatch(string userAgent)
		{
			if (string.IsNullOrEmpty(userAgent) || string.IsNullOrEmpty(Value))
			{
				return false;
			}

			var matchType = string.IsNullOrWhiteSpace(MatchType) ? MatchTypeContains : MatchType.Trim();

			if (matchType.Equals(MatchTypeExactMatch, StringComparison.OrdinalIgnoreCase))
			{
				return userAgent.Equals(Value, StringComparison.OrdinalIgnoreCase);
			}

			if (matchType.Equals(MatchTypeStartsWith, StringComparison.OrdinalIgnoreCase))
			{
				return userAgent.StartsWith(Value, StringComparison.OrdinalIgnoreCase);
			}

			// unknown match types fall back to Contains so a misconfigured rule still suppresses webp rather than silently serving it
			return userAgent.IndexOf(Value, StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}
}
