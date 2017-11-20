using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dianoga.Processors;

namespace Dianoga.Optimizers.Pipelines.DianogaWebP
{
	public class WebPOptimizer : CommandLineToolOptimizer
	{
		public override void Process(OptimizerArgs args)
		{
			//If WebP optimization was executed then abort running other optimizers
			//because they don't accept webp input file format
			if (args.AcceptWebP)
			{
				base.Process(args);
				args.AbortPipeline();
			}
		}

		protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
		{
			return $"\"{tempFilePath}\" -o \"{tempOutputPath}\" ";
		}
	}
}
