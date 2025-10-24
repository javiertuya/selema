using NLog;
using Giis.Portable.Util;
using Giis.Selema.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services.Impl
{
    /// <summary>
    /// Custom logging to produce the Selema html log file that emulates the output produced by standard loggers.
    /// Not using standard loggers to avoid interferences, but also issues calls to the current logger
    /// </summary>
    public class SelemaLogger : ISelemaLogger
    {
        private static readonly string SPAN_HTML = "<br/><span style=\"font-family:monospace;\">";
        private static readonly string SPAN_RED = "<span style=\"color:red;\">";
        private static readonly string SPAN_RED_BOLD = "<span style=\"color:red;font-weight:bold;\">";
        private static readonly string SPAN_END = "</span>";
        private readonly Logger log;
        private string loggerName;
        private string reportDir;
        private string logFile;
        public SelemaLogger(string loggerName, string reportDir, string logFileName)
        {
            this.log = LogManager.GetLogger(loggerName);
            this.loggerName = loggerName;
            this.reportDir = reportDir;
            this.logFile = FileUtil.GetPath(this.reportDir, logFileName);

            //ensures there is a directory for the logger
            FileUtil.CreateDirectory(reportDir);
            Info("Logger file created at: " + logFile);
        }

        public virtual void Trace(string message)
        {
            log.Trace(message);
        }

        public virtual void Debug(string message)
        {
            log.Debug(message);
        }

        public virtual void Info(string message)
        {
            Write("INFO", message);
            log.Info(message);
        }

        public virtual void Warn(string message)
        {
            Write("WARN", SPAN_RED + message + SPAN_END);
            Giis.Portable.Util.NLogUtil.Warn(log, message);
        }

        public virtual void Error(string message)
        {
            Write("ERROR", SPAN_RED_BOLD + message + SPAN_END);
            Giis.Portable.Util.NLogUtil.Error(log, message);
        }

        //only for testing
        public static string ReplaceTags(string logLine)
        {
            return logLine.Replace(SPAN_HTML, "").Replace(SPAN_RED_BOLD, "").Replace(SPAN_RED, "").Replace(SPAN_END, "");
        }

        private void Write(string type, string message)
        {
            string time = JavaCs.GetTime(JavaCs.GetCurrentDate());

            // some messages may have more than one line, replace line break by html break
            message = message.Replace("\r", "").Replace("\n", "<br/>");
            FileUtil.FileAppend(logFile, "\n" + Html("[" + type + "] " + time + " " + loggerName + " - " + message));
        }

        private string Html(string message)
        {
            return SPAN_HTML + message + SPAN_END;
        }
    }
}