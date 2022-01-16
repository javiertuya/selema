/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Portable;
using Giis.Selema.Services;
using NLog;
using Sharpen;

namespace Giis.Selema.Services.Impl
{
	/// <summary>Custom logging to produce the Selema html log file that emulates the output produced by standard loggers.</summary>
	/// <remarks>
	/// Custom logging to produce the Selema html log file that emulates the output produced by standard loggers.
	/// Not using standard loggers to avoid interferences, but also issues calls to the current logger
	/// </remarks>
	public class SelemaLogger : ISelemaLogger
	{
		private const string SpanHtml = "<br/><span style=\"font-family:monospace;\">";

		private const string SpanRed = "<span style=\"color:red;\">";

		private const string SpanRedBold = "<span style=\"color:red;font-weight:bold;\">";

		private const string SpanEnd = "</span>";

		private readonly Logger syslog;

		private string loggerName;

		private string reportDir;

		private string logFile;

		public SelemaLogger(string loggerName, string reportDir, string logFileName)
		{
			this.syslog = LogManager.GetLogger(loggerName);
			this.loggerName = loggerName;
			this.reportDir = reportDir;
			this.logFile = FileUtil.GetPath(this.reportDir, logFileName);
			//ensures there is a directory for the logger
			FileUtil.CreateDirectory(reportDir);
			Info("Logger file created at: " + logFile);
		}

		public virtual void Trace(string message)
		{
			syslog.Trace(message);
		}

		public virtual void Debug(string message)
		{
			syslog.Debug(message);
		}

		public virtual void Info(string message)
		{
			Write("INFO", message);
			syslog.Info(message);
		}

		public virtual void Warn(string message)
		{
			Write("WARN", SpanRed + message + SpanEnd);
			syslog.Warn(message);
		}

		public virtual void Error(string message)
		{
			Write("ERROR", SpanRedBold + message + SpanEnd);
			syslog.Error(message);
		}

		//only for testing
		public static string ReplaceTags(string logLine)
		{
			return logLine.Replace(SpanHtml, string.Empty).Replace(SpanRedBold, string.Empty).Replace(SpanRed, string.Empty).Replace(SpanEnd, string.Empty);
		}

		private void Write(string type, string message)
		{
			string time = JavaCs.GetTime(JavaCs.GetCurrentDate());
			FileUtil.FileAppend(logFile, "\n" + Html("[" + type + "] " + time + " " + loggerName + " - " + message));
		}

		private string Html(string message)
		{
			return SpanHtml + message + SpanEnd;
		}
	}
}
