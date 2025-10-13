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
    /// This browser service video recording works differently than the Selenoid and Selenium dynamic grid, because here, the
    /// browser server is composed by a preloaded browser server and a sidecar video server. The video server records video
    /// while is active, that would create large videos including all selenium sessions.
    /// 
    /// To overcome this problem, when calling the setVideo method, this service receives an instance of IVideoController
    /// that manages the start/stop of the video recording when the selenium session is open/closed.
    /// </summary>
    public class RemoteBrowserService : AbstractBrowserService
    {
        private IVideoController videoController;
        public override IBrowserService SetVideo()
        {
            throw new NotSupportedException("RemoteBrowserService requires an IVideoController " + "to start and stop video recording before and after the driver instantiation");
        }

        public virtual IBrowserService SetVideo(IVideoController videoController)
        {
            this.recordVideo = true;
            this.videoController = videoController;
            return this;
        }

        public override IVideoService GetVideoRecorder()
        {
            return recordVideo ? new RemoteVideoService(videoController) : null;
        }
    }
}