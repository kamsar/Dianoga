using System.Collections.Generic;
using System.Reflection;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Sites;
using Sitecore.StringExtensions;

namespace Dianoga
{
	/// <summary>
	/// This version of Sitecore's MediaCache also optimizes the images after they go into cache
	/// This effectively means that the first request for an image is NOT optimized (but it will be scaled, etc), 
	/// however subsequent requests will receive the optimized version from cache.
	/// </summary>
	public class OptimizingMediaCache : MediaCache
	{
		private readonly MediaOptimizer _optimizer;
		private static readonly HashSet<string> StreamsInOptimization = new HashSet<string>();
		private static readonly object OptimizeLock = new object();

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

			MediaCacheRecord cacheRecord = CreateCacheRecord(media, options, stream);

			if (cacheRecord == null) return false;

			cachedStream = cacheRecord.GetStream();

			if (cachedStream == null) return false;

			AddToActiveList(cacheRecord);
			/* END STOCK */

			// we store the site context because on the background thread: without the Sitecore context saved (on a worker thread), that disables the media cache
			var currentSite = Context.Site;

			cacheRecord.PersistAsync((() => OnAfterPersist(cacheRecord, currentSite)));
			
			return true;
		}

		protected virtual void OnAfterPersist(MediaCacheRecord record, SiteContext originalSiteContext)
		{
			RemoveFromActiveList(record);

			// Housekeeping: since we call AddStream() to insert the optimized version, we have to keep AddStream() from calling OnAfterPersist() from that call, causing an optimization loop
			var id = record.Media.MediaData.MediaId;

			if (StreamsInOptimization.Contains(id))
			{
				lock (OptimizeLock)
				{
					if (StreamsInOptimization.Contains(id))
					{
						StreamsInOptimization.Remove(id);
						return;
					}
				}
			}

			lock (OptimizeLock)
			{
				StreamsInOptimization.Add(id);
			}

			if (!record.HasStream) return;

			var stream = record.GetStream();

			if (!_optimizer.CanOptimize(stream)) return;

			var optimizedStream = _optimizer.Process(stream, record.Options);

			if (optimizedStream == null)
			{
				Log.Info("Dianoga: async optimazation result was null, not doing any optimizing for {0}".FormatWith(optimizedStream.MediaItem.MediaPath), this);
				return;
			}

			using (new SiteContextSwitcher(originalSiteContext))
			{
				// only here to satisfy out param
				MediaStream dgafStream;

				bool success = AddStream(record.Media, record.Options, optimizedStream, out dgafStream);
				if(dgafStream != null) dgafStream.Dispose();

				if (!success)
					 Log.Warn("Dianoga: The media cache rejected adding {0}. This is unexpected!".FormatWith(optimizedStream.MediaItem.MediaPath), this);
			}

		}

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
