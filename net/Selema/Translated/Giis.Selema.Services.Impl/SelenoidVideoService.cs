/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Giis.Selema.Portable;
using Giis.Selema.Services;
using OpenQA.Selenium;
using Sharpen;

namespace Giis.Selema.Services.Impl
{
	/// <summary>
	/// Support for video recording from a selenoid service: Selenoid produces a video per each driver session,
	/// this service keeps track of the video names, links to videos from the logs and timestamps when failures are produced
	/// </summary>
	public class SelenoidVideoService : IVideoService
	{
		private ISelemaLogger log;

		private const string VideoIndexName = "video-index.log";

		private long lastSessionStartingTimestamp = 0;

		private long lastSessionStartedTimestamp = 0;

		//los timestamps no se miden de forma precisa, pero se tomara como referencia el intervalo que se conoce
		/// <summary>Configures this service, called on attaching the service to a SeleManager</summary>
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
			{
				log.Info(videoMsg);
			}
			return videoMsg;
		}

		public virtual IDictionary<string, object> GetSeleniumOptions(IMediaContext context, string testName)
		{
			string videoFileName = context.GetVideoFileName(testName);
			Dictionary<string, object> opts = new Dictionary<string, object>();
			opts["enableVideo"] = true;
			opts["videoName"] = videoFileName;
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
			return (minutes < 10 ? "0" : string.Empty) + minutes + ":" + (seconds < 10 ? "0" : string.Empty) + seconds;
		}

		public virtual void BeforeQuitDriver(IMediaContext context, string testName)
		{
			string videoFileName = context.GetVideoFileName(testName);
			if (log != null)
			{
				log.Info("Saving video: " + "<a href=\"" + videoFileName + "\">" + videoFileName + "</a>");
			}
			string videoIndex = FileUtil.GetPath(context.GetReportFolder(), VideoIndexName);
			FileUtil.FileAppend(videoIndex, videoFileName + "\n");
		}
	}
}
