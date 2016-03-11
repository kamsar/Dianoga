using System;
using System.IO;

namespace Dianoga.Optimizers
{
	public abstract class OptimizerProcessor
	{
		public virtual void Process(OptimizerArgs args)
		{
			if (!ValidateInputStream(args)) return;

			var originalStream = new MemoryStream();

			try
			{
				// buffer the original stream to memory, so that if optimization fails to save file size we can back it out easily
				// and keep the original stream
				args.Stream.Seek(0, SeekOrigin.Begin);
				args.Stream.CopyTo(originalStream);
				originalStream.Seek(0, SeekOrigin.Begin);

				args.Stream.Seek(0, SeekOrigin.Begin);

				ProcessOptimizer(args);

				ValidateReturnStream(args, originalStream);
			}
			catch
			{
				// make sure we dispose all the streams on exception and rethrow the error
				try
				{
					originalStream.Dispose();
					args.Stream.Dispose();
				}
				catch { }

				throw;
			}
		}

		protected virtual bool ValidateInputStream(OptimizerArgs args)
		{
			return args.Stream != null && args.Stream.CanRead && args.Stream.CanSeek;
		}

		protected virtual void ValidateReturnStream(OptimizerArgs args, Stream originalStream)
		{
			if (args.Stream != null)
			{
				if (args.Stream is FileStream) throw new InvalidOperationException($"{GetType().Name} returned a FileStream. This will leave orphaned files on disk after the stream is disposed. Don't do that. Return files as pre-buffered memory streams.");
				if (!args.Stream.CanSeek) throw new InvalidOperationException($"{GetType().Name} returned a non seekable stream. This is not allowed. Try buffering it into a memory stream first.");
				if (!args.Stream.CanRead)
				{
					args.AddMessage($"{GetType().Name} returned a non readable stream. This probably means it failed after disposing the input stream. We will keep the original stream.");
					args.Stream = originalStream;
				}

				// seek to the start so the next processor, if there is one, gets a clean stream
				args.Stream.Seek(0, SeekOrigin.Begin);

				if (args.Stream.Length > originalStream.Length)
				{
					args.Stream.Dispose();
					args.Stream = originalStream;
					args.AddMessage($"{GetType().Name}: the optimized image resulted in a larger file size ({args.Stream.Length} vs {originalStream.Length}). Using the original instead.");
				}
				else
				{
					originalStream.Dispose();
				}
			}
		}

		protected abstract void ProcessOptimizer(OptimizerArgs args);
	}
}
