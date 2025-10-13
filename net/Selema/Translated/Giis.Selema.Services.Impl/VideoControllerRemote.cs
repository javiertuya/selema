using NLog;
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
    /// Video controller to use when the recorder container and the tests run in different or the same VM and start/stop is
    /// managed by a separated server (video-controller).
    /// 
    /// The video-controller server to start the recorder (post), stop and download the video (get) and delete the recorded
    /// video to get ready for next session.
    /// </summary>
    public class VideoControllerRemote : IVideoController
    {
        readonly Logger log = LogManager.GetCurrentClassLogger();
        private string endpoint;
        private string targetFolder;
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="name">an identifier of the pair of selenium node and video recorder, that will be passed as a path parameter</param>
        /// <param name="controllerUrl">the endpoint of the video-controller</param>
        /// <param name="targetFolder">where the recorded videos will be stored after recording</param>
        public VideoControllerRemote(string name, string controllerUrl, string targetFolder)
        {
            this.endpoint = controllerUrl + "/" + name;
            this.targetFolder = targetFolder;
        }

        public virtual void Start()
        {
            RestClient.RestCall("POST", this.endpoint);
        }

        public virtual void Stop(string videoName)
        {
            string targetFile = FileUtil.GetPath(this.targetFolder, videoName);

            // Call the api to download and then delete the video file
            RestClient.RestDownload(this.endpoint, targetFile);
            RestClient.RestCall("DELETE", this.endpoint);
        }
    }
}