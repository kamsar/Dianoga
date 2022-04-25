using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Integration
{
	public class BaseTests
	{
		protected string CDHostname;
		protected bool SvgOptimizationEnabled;
		protected bool WebpOptimizationEnabled;
		protected bool AvifOptimizationEnabled;
		protected bool JxlOptimizationEnabled;

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
			WebpOptimizationEnabled = GetBoolValue(Constants.Variables.WebpOptimizationEnabled);
			JxlOptimizationEnabled = GetBoolValue(Constants.Variables.JxlOptimizationEnabled);
			AvifOptimizationEnabled = GetBoolValue(Constants.Variables.AvifOptimizationEnabled);
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

		protected string ResponseToString(WebResponse response)
		{
			var receiveStream = response.GetResponseStream();
			var readStream = new StreamReader(receiveStream, Encoding.UTF8);
			return readStream.ReadToEnd();
		}
	}
}
