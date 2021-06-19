using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks.Dataflow;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Sites;

namespace Dianoga.Invokers.MediaCacheAsync
{
	/// <summary>
	/// This version of Sitecore's MediaCache also optimizes the images after they go into cache
	/// This effectively means that the first request for an image is NOT optimized (but it will be scaled, etc), 
	/// however subsequent requests will receive the optimized version from cache.
	/// </summary>
	public class OptimizingMediaCache : MediaCache
	{
		private readonly MediaOptimizer _optimizer;
		private readonly ActionBlock<Action> _actionBlock;

		public OptimizingMediaCache(MediaOptimizer optimizer, ActionBlock<Action> actionBlock)
		{
			_optimizer = optimizer;
			_actionBlock = actionBlock;
		}

		public override bool AddStream(Media media, MediaOptions options, MediaStream stream, out MediaStream cachedStream)
		{
			Assert.ArgumentNotNull(media, "media");
			Assert.ArgumentNotNull(options, "options");
			Assert.ArgumentNotNull(stream, "stream");

			cachedStream = null;

			if (!CanCache(media, options))
				return false;

			if (string.IsNullOrEmpty(media.MediaData.MediaId))
				return false;

			if (!stream.Stream.CanRead)
			{
				DianogaLog.Warn($"Cannot optimize {media.MediaData.MediaItem.MediaPath} because cache was passed a non readable stream.");
				return false;
			}

			// buffer the stream if it's say a SQL stream
			stream.MakeStreamSeekable();
			stream.Stream.Seek(0, SeekOrigin.Begin);

			// Sitecore will use this to stream the media while we persist
			cachedStream = stream;

			// we store the site context because on the background thread: without the Sitecore context saved (on a worker thread), that disables the media cache
			var currentSite = Context.Site;


			// use an action block to ensure only the configured number of threads will try and optimize
			_actionBlock.Post(() => Optimize(currentSite, media, options));

			return true;
		}

		private void Optimize(SiteContext currentSite, Media media, MediaOptions options)
		{
			var mediaItem = media.MediaData.MediaItem;

			MediaStream originalMediaStream = null;
			MediaStream backupMediaStream = null;
			MediaStream optimizedMediaStream = null;

			try
			{
				// switch to the right site context (see above)
				using (new SiteContextSwitcher(currentSite))
				{
					//if the image is already optimized, abort task
					if (this.Contains(media, options))
					{
						return;
					}

					//get stream from mediaItem to reduce memory usage
					using (var stream = media.GetStream(options))
					{
						// make a copy of the stream to use
						var originalStream = new MemoryStream();
						stream.CopyTo(originalStream);
						originalStream.Seek(0, SeekOrigin.Begin);

						originalMediaStream = new MediaStream(originalStream, media.Extension, mediaItem);

						// make a stream backup we can use to persist in the event of an optimization failure
						// (which will dispose of originalStream)
						var backupStream = new MemoryStream();
						originalStream.CopyTo(backupStream);
						backupStream.Seek(0, SeekOrigin.Begin);

						backupMediaStream = new MediaStream(backupStream, media.Extension, mediaItem);
					}

					MediaCacheRecord cacheRecord = null;

					optimizedMediaStream = _optimizer.Process(originalMediaStream, options);

					if (optimizedMediaStream == null)
					{
						DianogaLog.Info($"Dianoga: {mediaItem.MediaPath} cannot be optimized due to media type or path exclusion");
						cacheRecord = CreateCacheRecord(media, options, backupMediaStream);
					}

					if (cacheRecord == null)
					{
						cacheRecord = CreateCacheRecord(media, options, optimizedMediaStream);
					}

					AddToActiveList(cacheRecord);

					cacheRecord.Persist();

					RemoveFromActiveList(cacheRecord);
				}
			}
			catch (Exception ex)
			{
				// this runs in a background thread, and an exception here would cause IIS to terminate the app pool. Bad! So we catch/log, just in case.
				DianogaLog.Error($"Dianoga: Exception occurred on the background thread when optimizing: {mediaItem.MediaPath}", ex);
			}
			finally
			{
				// release resources used by the optimization task
				originalMediaStream?.Dispose();
				backupMediaStream?.Dispose();
				optimizedMediaStream?.Dispose();
			}
		}

		// the 'active list' is an internal construct that lets Sitecore stream media to the client at the same time as it's being written to cache
		// unfortunately though the rest of MediaCache is virtual, these methods are inexplicably not
		protected virtual void AddToActiveList(MediaCacheRecord record)
		{
			var baseMethod = typeof(MediaCache).GetMethod("AddToActiveList", BindingFlags.Instance | BindingFlags.NonPublic);

			if (baseMethod != null)
				baseMethod.Invoke(this, new object[] { record });
			else DianogaLog.Error("Dianoga: Couldn't use malevolent private reflection on AddToActiveList! This may mean Dianoga isn't entirely compatible with this version of Sitecore, though it should only affect a performance optimization.");

			// HEY SITECORE, CAN WE GET THESE VIRTUAL? KTHX.
		}

		protected virtual void RemoveFromActiveList(MediaCacheRecord record)
		{
			var baseMethod = typeof(MediaCache).GetMethod("RemoveFromActiveList", BindingFlags.Instance | BindingFlags.NonPublic);

			if (baseMethod != null)
				baseMethod.Invoke(this, new object[] { record });
			else DianogaLog.Error("Dianoga: Couldn't use malevolent private reflection on RemoveFromActiveList! This may mean Dianoga isn't entirely compatible with this version of Sitecore, though it should only affect a performance optimization.");
		}
	}
}
