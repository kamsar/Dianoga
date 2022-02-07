using Microsoft.Extensions.Configuration;

namespace Integration
{
	public class BaseTests
	{
		protected string CDHostname;
		protected bool SvgOptimizationEnabled;
		public BaseTests()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables()
				.Build();

			CDHostname = config.GetValue<string>("CDHostname");
			SvgOptimizationEnabled = config.GetValue<bool>("SvgOptimizationEnabled");
		}
	}
}
