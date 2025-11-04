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
            long timestamp = JavaCs.CurrentTimeMillis();

            // As recording starts here, the times to determine the failure window must be overwritten
            lastSessionStartingTimestamp = JavaCs.CurrentTimeMillis();
            VideoControllerStart();
            lastSessionStartedTimestamp = JavaCs.CurrentTimeMillis();
            log.Trace("Time to start recorder: " + (JavaCs.CurrentTimeMillis() - timestamp) + "ms");
        }

        public override void BeforeQuitDriver(IMediaContext context, string testName)
        {
            string videoFileName = GetVideoFileNameWithRelativePath(context, testName);
            long timestamp = JavaCs.CurrentTimeMillis();
            VideoControllerStop(videoFileName);
            log.Trace("Time to stop recorder: " + (JavaCs.CurrentTimeMillis() - timestamp) + "ms");
            base.BeforeQuitDriver(context, testName);
        }

        private void VideoControllerStart()
        {
            try
            {
                videoController.Start();
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
                videoController.Stop(videoFileName);
            }
            catch (Exception e)
            {

                //
                logSelema.Error("Video controller stop failure - " + e.Message);
            }
        }
    }
}