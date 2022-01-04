namespace Dianoga.NextGenFormats.Pipelines.DianogaGetSupportedFormats
{
	public class CheckSupport
	{
		public virtual string Extension
		{
			get;
			set;
		}

		public void Process(SupportedFormatsArgs args)
		{
			var supports = args.Input.Contains($"{args.Suffix}{Extension}"); ;
			if (supports)
			{
				args.Extensions.Add(Extension);
			}
		}
	}
}
