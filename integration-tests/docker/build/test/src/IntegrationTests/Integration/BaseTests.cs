using System;
using Microsoft.Extensions.Configuration;

namespace Integration
{
	public class BaseTests
	{
		protected string CDHostname;
		protected bool SvgOptimizationEnabled;

		protected bool Async;

		protected bool Sync => !Async;

		protected IConfigurationRoot ConfigurationRoot;

		public BaseTests()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables()
				.Build();

			ConfigurationRoot = config;

			CDHostname = GetStringSetting(Constants.Variables.CDHostname);
			SvgOptimizationEnabled = GetBoolValue(Constants.Variables.SvgOptimizationEnabled);
			Async = GetBoolValue(Constants.Variables.Async);
		}

		protected string GetStringSetting(string name)
		{
			return Environment.GetEnvironmentVariable(name)
			       ?? ConfigurationRoot.GetValue<string>(name);
		}

		protected bool GetBoolValue(string name)
		{
			var val = Environment.GetEnvironmentVariable(name);
			return !string.IsNullOrEmpty(val)
				? bool.Parse(val)
				: ConfigurationRoot.GetValue<bool>(name);
		}
	}
}
