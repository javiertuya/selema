using OpenQA.Selenium;
using NLog;
using Giis.Portable.Util;
using Giis.Selema.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services.Browser
{
    /// <summary>
    /// Manages the start/stop of the video recording container when the selenium session is open/closed (driver
    /// created/closed).
    /// </summary>
    public class RemoteVideoService : AbstractVideoService
    {
        readonly Logger log = LogManager.GetCurrentClassLogger(); //general purpose logger
        // For preloaded services, an additional instance for controller the video recording is required
        private IVideoController videoController;
        public RemoteVideoService(IVideoController videoController)
        {
            this.videoController = videoController;
        }

        public override void AfterCreateDriver(IWebDriver driver)
        {
            base.AfterCreateDriver(driver);
            VideoControllerStart();
        }

        public override void BeforeQuitDriver(IMediaContext context, string testName)
        {
            string videoFileName = GetVideoFileNameWithRelativePath(context, testName);
            VideoControllerStop(videoFileName);
            base.BeforeQuitDriver(context, testName);
        }

        private void VideoControllerStart()
        {
            try
            {
                long timestamp = JavaCs.CurrentTimeMillis();
                videoController.Start();
                logSelema.Warn("Time to start recording " + (JavaCs.CurrentTimeMillis() - timestamp) + "ms");
            }
            catch (Exception e)
            {

                //
                logSelema.Error("Video controller start failure - " + e.Message);
            }
        }

        private void VideoControllerStop(string videoFileName)
        {
            try
            {
                long timestamp = JavaCs.CurrentTimeMillis();
                videoController.Stop(videoFileName);
                logSelema.Warn("Time to stop recording " + (JavaCs.CurrentTimeMillis() - timestamp) + "ms");
            }
            catch (Exception e)
            {

                //
                logSelema.Error("Video controller stop failure - " + e.Message);
            }
        }
    }
}