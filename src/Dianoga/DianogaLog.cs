using System;
using log4net;
using Sitecore;
using Sitecore.Diagnostics;

namespace Dianoga
{

	/// <summary>
	/// Dianoga Logger
	/// </summary>
	public static class DianogaLog
	{
		/// <summary>
		/// The local logger instance.
		/// </summary>
		private static readonly ILog Log;

		/// <summary>
		/// Initializes static members of the <see cref="T:Dianoga.DianogaLog" /> class. 
		/// </summary>
		static DianogaLog()
		{
			Log = LogManager.GetLogger("Dianoga") ?? LoggerFactory.GetLogger(typeof(DianogaLog));
		}

		/// <summary>
		/// Logs info message with context user name.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Audit(string message)
		{
			Assert.ArgumentNotNull(message, "message");
			Log.Info($"AUDIT ({Context.User.Name}) {message}");
		}

		/// <summary>
		/// Logs error message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		public static void Error(string message, Exception exception = null)
		{
			Assert.IsNotNull(Log, "Logger implementation was not initialized");
			if (exception == null)
			{
				Log.Error(message);
			}
			else
			{
				Log.Error(message, exception);
			}
		}

		/// <summary>
		/// Logs information message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		public static void Info(string message, Exception exception = null)
		{
			Assert.IsNotNull(Log, "Logger implementation was not initialized");
			if (exception == null)
			{
				Log.Info(message);
			}
			else
			{
				Log.Info(message, exception);
			}
		}

		/// <summary>
		/// Logs warning message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		public static void Warn(string message, Exception exception = null)
		{
			Assert.IsNotNull(Log, "Logger implementation was not initialized");
			if (exception == null)
			{
				Log.Warn(message);
			}
			else
			{
				Log.Warn(message, exception);
			}
		}

		/// <summary>
		/// Logs fatal error message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		public static void Fatal(string message, Exception exception = null)
		{
			Assert.IsNotNull(Log, "Logger implementation was not initialized");
			if (exception == null)
			{
				Log.Fatal(message);
			}
			else
			{
				Log.Fatal(message, exception);
			}
		}

		/// <summary>
		/// Logs debug message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		public static void Debug(string message, Exception exception = null)
		{
			Assert.IsNotNull(Log, "Logger implementation was not initialized");
			if (exception == null)
			{
				Log.Debug(message);
			}
			else
			{
				Log.Debug(message, exception);
			}
		}
	}

}
