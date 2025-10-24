using OpenQA.Selenium;
using Giis.Portable.Util;
using Giis.Selema.Portable.Selenium;
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
    /// Screenshot management
    /// </summary>
    public class ScreenshotService : IScreenshotService
    {
        private ISelemaLogger logSelema;
        /// <summary>
        /// Configures this service, called on attaching the service to a SeleManager
        /// </summary>
        public virtual IScreenshotService Configure(ISelemaLogger thisLog)
        {
            logSelema = thisLog;
            return this;
        }

        /// <summary>
        /// Takes an picture of the current state of the browser and saves it to the reports folder
        /// </summary>
        public virtual string TakeScreenshot(IWebDriver driver, IMediaContext context, string testName)
        {
            string screenshotFile = context.GetScreenshotFileName(testName);
            try
            {
                string fileName = FileUtil.GetPath(context.GetReportFolder(), screenshotFile);
                string screenshotUrl = "<a href=\"" + screenshotFile + "\">" + screenshotFile + "</a>";
                string msg = "Taking screenshot: " + screenshotUrl;
                if (logSelema != null)
                    logSelema.Info(msg);
                SeleniumActions.TakeScreenshotToFile(driver, fileName);
                return msg;
            }
            catch (Exception e)
            {
                string msg = "Can't take screenshot or write the content to file " + screenshotFile + ". Message: " + e.Message;
                if (logSelema != null)
                    logSelema.Error(msg);
                return msg;
            }
        }
    }
}