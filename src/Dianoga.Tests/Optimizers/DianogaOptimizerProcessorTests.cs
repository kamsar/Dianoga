using System;
using System.IO;
using Dianoga.Optimizers;
using FluentAssertions;
using Xunit;

namespace Dianoga.Tests.Optimizers
{
	public class DianogaOptimizerProcessorTests
	{
		[Fact]
		public void ShouldNotProcess_WhenInputStreamIsNull()
		{
			var args = new OptimizerArgs(null);
			var processor = new TestOptimizerProcessor(optimizerArgs => { throw new Exception(); });

			processor.Process(args); // would throw if body called
		}

		[Fact]
		public void ShouldNotProcess_WhenInputStreamIsDisposed()
		{
			var stream = new MemoryStream();
			var args = new OptimizerArgs(stream);
			stream.Dispose();

			var processor = new TestOptimizerProcessor(optimizerArgs => { throw new Exception(); });

			processor.Process(args); // would throw if body called
		}

		[Fact]
		public void ShouldReturnStream_AtPositionZero()
		{
			var stream = new MemoryStream(new byte[] { 12, 13, 14 });
			var args = new OptimizerArgs(stream);

			var processor = new TestOptimizerProcessor(optimizerArgs =>
			{
				optimizerArgs.Stream.Close();
				optimizerArgs.Stream = new MemoryStream(new byte[] { 13, 14 }); // need to return a shorter stream or it will get tripped
				optimizerArgs.Stream.Seek(0, SeekOrigin.End); // seek return stream to the end intentionally
			});

			processor.Process(args);

			args.Stream.Position.Should().Be(0);
			args.Stream.Dispose();
		}

		[Fact]
		public void ShouldReturnOriginalStream_WhenOptimizedIsLonger()
		{
			var stream = new MemoryStream(new byte[] { 12 });
			var args = new OptimizerArgs(stream);

			var processor = new TestOptimizerProcessor(optimizerArgs =>
			{
				optimizerArgs.Stream.Close();
				optimizerArgs.Stream = new MemoryStream(new byte[] { 13, 14 }); // note longer than input stream
			});

			processor.Process(args);

			args.Stream.Length.Should().Be(1);
			args.GetMessages().Length.Should().Be(1);
			args.Stream.Dispose();
		}

		[Fact]
		public void ShouldDisposeStreams_WhenProcessorThrowsException()
		{
			var stream = new MemoryStream(new byte[] { 12 });
			var args = new OptimizerArgs(stream);

			var processor = new TestOptimizerProcessor(optimizerArgs => { throw new Exception(); });

			try
			{
				processor.Process(args);
			}
			catch
			{
				stream.CanRead.Should().BeFalse();
			}
		}
	}
}
