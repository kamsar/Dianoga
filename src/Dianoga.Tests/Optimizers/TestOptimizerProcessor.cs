using System;
using Dianoga.Optimizers;

namespace Dianoga.Tests.Optimizers
{
	public class TestOptimizerProcessor : OptimizerProcessor
	{
		private readonly Action<OptimizerArgs> _bodyAction;

		public TestOptimizerProcessor(Action<OptimizerArgs> bodyAction)
		{
			_bodyAction = bodyAction;
		}

		protected override void ProcessOptimizer(OptimizerArgs args)
		{
			_bodyAction(args);
		}
	}
}
