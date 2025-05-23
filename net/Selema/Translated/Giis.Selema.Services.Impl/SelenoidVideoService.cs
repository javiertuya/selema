using Java.Util;
using OpenQA.Selenium;
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
    /// Support for video recording from a selenoid service: Selenoid produces a video per each driver session,
    /// this service keeps track of the video names, links to videos from the logs and timestamps when failures are produced
    /// </summary>
    public class SelenoidVideoService : IVideoService
    {
        private ISelemaLogger log;
        private static readonly string VIDEO_INDEX_NAME = "video-index.log";
        //los timestamps no se miden de forma precisa, pero se tomara como referencia el intervalo que se conoce
        private long lastSessionStartingTimestamp = 0;
        private long lastSessionStartedTimestamp = 0;
        /// <summary>
        /// Configures this service, called on attaching the service to a SeleManager
        /// </summary>
        public virtual IVideoService Configure(ISelemaLogger thisLog)
        {
            log = thisLog;
            return this;
        }

        public virtual void BeforeCreateDriver()
        {
            lastSessionStartingTimestamp = JavaCs.CurrentTimeMillis();
        }

        public virtual void AfterCreateDriver(IWebDriver driver)
        {
            lastSessionStartedTimestamp = JavaCs.CurrentTimeMillis();
        }

        public virtual string OnTestFailure(IMediaContext context, string testName)
        {
            long nowTimestamp = JavaCs.CurrentTimeMillis();
            string videoFileName = context.GetVideoFileName(testName);
            string videoUrl = "<a href=\"" + videoFileName + "\">" + videoFileName + "</a>";
            string videoMsg = "Recording video at " + GetSessionTimestamp(nowTimestamp) + " (aprox.): " + videoUrl;
            if (log != null)
                log.Info(videoMsg);
            return videoMsg;
        }

        public virtual Map<string, object> GetSeleniumOptions(IMediaContext context, string testName)
        {
            string videoFileName = context.GetVideoFileName(testName);
            Map<string, object> opts = new HashMap<string, object>(); // NOSONAR net compatibility
            opts.Put("enableVideo", true);
            opts.Put("videoName", videoFileName);
            return opts;
        }

        private string GetSessionTimestamp(long nowTimestamp)
        {
            long starting = (nowTimestamp - lastSessionStartingTimestamp) / 1000;
            long started = (nowTimestamp - lastSessionStartedTimestamp) / 1000;
            return "[" + FormatSeconds(started) + " " + FormatSeconds(starting) + "]";
        }

        private string FormatSeconds(long totalSeconds)
        {
            long seconds = totalSeconds % 60;
            long minutes = totalSeconds / 60;
            return (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
        }

        public virtual void BeforeQuitDriver(IMediaContext context, string testName)
        {
            string videoFileName = context.GetVideoFileName(testName);
            if (log != null)
                log.Info("Saving video: " + "<a href=\"" + videoFileName + "\">" + videoFileName + "</a>");
            string videoIndex = FileUtil.GetPath(context.GetReportFolder(), VIDEO_INDEX_NAME);
            FileUtil.FileAppend(videoIndex, videoFileName + "\n");
        }
    }
}