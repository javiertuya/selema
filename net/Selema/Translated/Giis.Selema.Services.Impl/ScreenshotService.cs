/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Giis.Portable.Util;
using Giis.Selema.Portable.Selenium;
using Giis.Selema.Services;
using OpenQA.Selenium;
using Sharpen;

namespace Giis.Selema.Services.Impl
{
	/// <summary>Screenshot management</summary>
	public class ScreenshotService : IScreenshotService
	{
		private ISelemaLogger log;

		/// <summary>Configures this service, called on attaching the service to a SeleManager</summary>
		public virtual IScreenshotService Configure(ISelemaLogger thisLog)
		{
			log = thisLog;
			return this;
		}

		/// <summary>Takes an picture of the current state of the browser and saves it to the reports folder</summary>
		public virtual string TakeScreenshot(IWebDriver driver, IMediaContext context, string testName)
		{
			string screenshotFile = context.GetScreenshotFileName(testName);
			try
			{
				string fileName = FileUtil.GetPath(context.GetReportFolder(), screenshotFile);
				string screenshotUrl = "<a href=\"" + screenshotFile + "\">" + screenshotFile + "</a>";
				string msg = "Taking screenshot: " + screenshotUrl;
				if (log != null)
				{
					log.Info(msg);
				}
				SeleniumActions.TakeScreenshotToFile(driver, fileName);
				return msg;
			}
			catch (Exception e)
			{
				string msg = "Can't take screenshot or write the content to file " + screenshotFile + ". Message: " + e.Message;
				if (log != null)
				{
					log.Error(msg.Replace("\n", "<br/>"));
				}
				//some messages may have more than one line
				return msg;
			}
		}
	}
}
