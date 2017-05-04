using System;
using log4net;

namespace YetAnotherFlickrUploader.Helpers
{
	public static class Logger
	{
		private static readonly ILog Log = LogManager.GetLogger("app");

		public static void Debug(string message)
		{
			ConsoleHelper.WriteDebugLine(message);
			Log.Debug(FormatMessage(message));
		}

		public static void Debug(string format, params object[] args)
		{
			ConsoleHelper.WriteDebugLine(format, args);
			Log.Debug(FormatMessage(format, args));
		}

		public static void Info(string message)
		{
			ConsoleHelper.WriteInfoLine(message);
			Log.Info(FormatMessage(message));
		}

		public static void Info(string format, params object[] args)
		{
			ConsoleHelper.WriteInfoLine(format, args);
			Log.Info(FormatMessage(format, args));
		}

		public static void Warning(string message)
		{
			ConsoleHelper.WriteWarningLine(message);
			Log.Warn(FormatMessage(message));
		}

		public static void Warning(string format, params object[] args)
		{
			ConsoleHelper.WriteWarningLine(format, args);
			Log.Warn(FormatMessage(format, args));
		}

		public static void Error(string message)
		{
			ConsoleHelper.WriteErrorLine(message);
			Log.Error(FormatMessage(message));
		}

		public static void Error(string format, params object[] args)
		{
			ConsoleHelper.WriteErrorLine(format, args);
			Log.Error(FormatMessage(format, args));
		}

		public static void Error(string message, Exception e)
		{
			ConsoleHelper.WriteErrorLine(message);
			ConsoleHelper.WriteException(e);
			Log.Error(FormatMessage(message), e);
		}

		private static string FormatMessage(string format, params object[] args)
		{
			var result = args != null && args.Length > 0 ? string.Format(format, args) : format;
			return result.TrimEnd(' ');
		}
	}
}
