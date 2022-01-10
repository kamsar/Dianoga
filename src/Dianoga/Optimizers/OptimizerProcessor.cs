using System;
using System.IO;
using Sitecore.Diagnostics;

namespace Dianoga.Optimizers
{
	public abstract class OptimizerProcessor
	{
		// NOTE: IT IS EXPECTED THAT ANY PROCESSOR WILL DISPOSE OF THE INPUT STREAM ONCE IT CONSUMES IT
		// Throw an exception if you have a processing error.
		public virtual void Process(OptimizerArgs args)
		{
			if (!ValidateInputStream(args)) return;

			var originalStream = new MemoryStream();

			// if we cannot seek the stream, we buffer it into a memory stream so we can seek it
			// this can occur if getting an unresized image in the getMediaStream pipeline
			if (!args.Stream.CanSeek)
			{
				var bufferedStream = new MemoryStream();
				args.Stream.CopyTo(bufferedStream);

				args.Stream.Dispose();
				args.Stream = bufferedStream;
			}

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
			catch (Exception ex)
			{
				args.IsOptimized = false;
				args.Stream.Dispose();
				args.Stream = originalStream;
				DianogaLog.Error($"Dianoga: Unable to optimize {args.MediaPath} due to a processing error! It will be unchanged.", ex);
			}
		}

		protected virtual bool ValidateInputStream(OptimizerArgs args)
		{
			return args.Stream != null && args.Stream.CanRead;
		}

		protected virtual void ValidateReturnStream(OptimizerArgs args, Stream originalStream)
		{
			args.IsOptimized = false; // Assume its not by default

			if (args.Stream != null && args.Stream.Length > 0)
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
					args.AddMessage($"{GetType().Name}: the optimized image resulted in a larger file size ({args.Stream.Length} vs {originalStream.Length}). Using the original instead.");
					args.Stream.Dispose();
					args.Stream = originalStream;
				}
				else if (args.Stream.Length == originalStream.Length)
				{
					args.AddMessage($"{GetType().Name}: the optimized image resulted in the same file size ({args.Stream.Length}). Using the original instead.");
					args.Stream.Dispose();
					args.Stream = originalStream;
				}
				else
				{
					originalStream.Dispose();
					args.IsOptimized = true;
				}
			}
			else
			{
				if (args.Stream == null)
					args.AddMessage($"{GetType().Name} returned a null stream. This probably means it failed. We will keep the original stream.");
				else
					args.AddMessage($"{GetType().Name} returned a zero length stream. This probably means it failed. We will keep the original stream.");

				args.Stream = originalStream;
			}
		}

		protected abstract void ProcessOptimizer(OptimizerArgs args);
	}
}
