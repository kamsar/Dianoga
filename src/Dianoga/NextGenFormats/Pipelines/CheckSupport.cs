using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Sitecore.Xml;

namespace Dianoga.NextGenFormats.Pipelines.DianogaGetSupportedFormats
{
	public class CheckSupport
	{
		public virtual string Extension
		{
			get;
			set;
		}

		public List<UserAgentRule> UnsupportedUserAgents { get; set; } = new List<UserAgentRule>();

		// invoked by the Sitecore config factory for each child of <UnsupportedUserAgents hint="raw:AddUnsupportedUserAgent">
		public void AddUnsupportedUserAgent(XmlNode configNode)
		{
			var value = XmlUtil.GetAttribute("value", configNode);
			if (string.IsNullOrEmpty(value))
			{
				return;
			}

			var matchType = XmlUtil.GetAttribute("matchType", configNode);
			UnsupportedUserAgents.Add(new UserAgentRule
			{
				Value = value,
				MatchType = string.IsNullOrEmpty(matchType) ? UserAgentRule.MatchTypeContains : matchType
			});
		}

		public void Process(SupportedFormatsArgs args)
		{
			if (IsUserAgentUnsupported(GetUserAgent()))
			{
				return;
			}

			var supports = args.Input.Contains($"{args.Prefix}{Extension}"); ;
			if (supports)
			{
				args.Extensions.Add(Extension);
			}
		}

		public virtual bool IsUserAgentUnsupported(string userAgent)
		{
			return UnsupportedUserAgents != null && UnsupportedUserAgents.Any(rule => rule != null && rule.IsMatch(userAgent));
		}

		protected virtual string GetUserAgent()
		{
			return HttpContext.Current?.Request?.UserAgent;
		}
	}
}
