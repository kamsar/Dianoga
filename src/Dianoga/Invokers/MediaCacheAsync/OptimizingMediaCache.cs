using System;
using System.IO;
using System.Reflection;
using System.Threading;
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

		public OptimizingMediaCache(MediaOptimizer optimizer)
		{
			_optimizer = optimizer;
		}

		public override bool AddStream(Media media, MediaOptions options, MediaStream stream, out MediaStream cachedStream)
		{
			/* STOCK METHOD (Decompiled) */
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
				Log.Warn($"Cannot optimize {media.MediaData.MediaItem.MediaPath} because cache was passed a non readable stream.", this);
				return false;
			}

			// buffer the stream if it's say a SQL stream
			stream.MakeStreamSeekable();
			stream.Stream.Seek(0, SeekOrigin.Begin);

			// Sitecore will use this to stream the media while we persist
			cachedStream = stream;

			// make a copy of the stream to use temporarily while we async persist
			var copyStream = new MemoryStream();
			stream.CopyTo(copyStream);
			copyStream.Seek(0, SeekOrigin.Begin);

			var copiedMediaStream = new MediaStream(copyStream, stream.Extension, stream.MediaItem);

			// we store the site context because on the background thread: without the Sitecore context saved (on a worker thread), that disables the media cache
			var currentSite = Context.Site;

			ThreadPool.QueueUserWorkItem(state =>
			{
				var mediaPath = stream.MediaItem.MediaPath;

				try
				{
					// make a stream backup we can use to persist in the event of an optimization failure
					// (which will dispose of copyStream)
					var backupStream = new MemoryStream();
					copyStream.CopyTo(backupStream);
					backupStream.Seek(0, SeekOrigin.Begin);

					var backupMediaStream = new MediaStream(backupStream, stream.Extension, stream.MediaItem);

					// switch to the right site context (see above)
					using (new SiteContextSwitcher(currentSite))
					{
						MediaCacheRecord cacheRecord = null;

						var optimizedStream = _optimizer.Process(copiedMediaStream, options);

						if (optimizedStream == null)
						{
							Log.Info($"Dianoga: {mediaPath} cannot be optimized due to media type or path exclusion", this);
							cacheRecord = CreateCacheRecord(media, options, backupMediaStream);
						}

						if (cacheRecord == null)
						{
							cacheRecord = CreateCacheRecord(media, options, optimizedStream);
							backupMediaStream.Dispose();
						}

						AddToActiveList(cacheRecord);

						cacheRecord.Persist();

						RemoveFromActiveList(cacheRecord);
					}
				}
				catch (Exception ex)
				{
					// this runs in a background thread, and an exception here would cause IIS to terminate the app pool. Bad! So we catch/log, just in case.
					Log.Error($"Dianoga: Exception occurred on the background thread when optimizing: {mediaPath}", ex, this);
				}
			});

			return true;
		}

		// the 'active list' is an internal construct that lets Sitecore stream media to the client at the same time as it's being written to cache
		// unfortunately though the rest of MediaCache is virtual, these methods are inexplicably not
		protected virtual void AddToActiveList(MediaCacheRecord record)
		{
			var baseMethod = typeof(MediaCache).GetMethod("AddToActiveList", BindingFlags.Instance | BindingFlags.NonPublic);

			if (baseMethod != null)
				baseMethod.Invoke(this, new object[] { record });
			else Log.Error("Dianoga: Couldn't use malevolent private reflection on AddToActiveList! This may mean Dianoga isn't entirely compatible with this version of Sitecore, though it should only affect a performance optimization.", this);

			// HEY SITECORE, CAN WE GET THESE VIRTUAL? KTHX.
		}

		protected virtual void RemoveFromActiveList(MediaCacheRecord record)
		{
			var baseMethod = typeof(MediaCache).GetMethod("RemoveFromActiveList", BindingFlags.Instance | BindingFlags.NonPublic);

			if (baseMethod != null)
				baseMethod.Invoke(this, new object[] { record });
			else Log.Error("Dianoga: Couldn't use malevolent private reflection on RemoveFromActiveList! This may mean Dianoga isn't entirely compatible with this version of Sitecore, though it should only affect a performance optimization.", this);
		}
	}
}
