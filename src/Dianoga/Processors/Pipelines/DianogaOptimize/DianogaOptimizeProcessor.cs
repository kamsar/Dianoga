namespace Dianoga.Processors.Pipelines.DianogaOptimize
{
	public abstract class DianogaOptimizeProcessor
	{
		public virtual void Process(ProcessorArgs args)
		{
			ProcessOptimize(args);
		}

		protected abstract void ProcessOptimize(ProcessorArgs args);
	}
}
