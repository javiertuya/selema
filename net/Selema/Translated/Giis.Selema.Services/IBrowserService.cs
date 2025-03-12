using Java.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services
{
    public interface IBrowserService
    {
        /// <summary>
        /// Activates the video recording, provided that the service is configured for video recording
        /// </summary>
        IBrowserService SetVideo();
        /// <summary>
        /// Activates the VNC capabilities to be able to watch the test execution in real time
        /// </summary>
        IBrowserService SetVnc();
        /// <summary>
        /// Adds a special capability other than the predefined video and vnc
        /// </summary>
        IBrowserService SetCapability(string key, object value);
        /// <summary>
        /// Gets the video recorder service associated with this browser service
        /// </summary>
        IVideoService GetVideoRecorder();
        /// <summary>
        /// Gets the capabilities that the IWebDriver must configure to integrate with this service
        /// </summary>
        Map<string, object> GetSeleniumOptions(string sessionName);
    }
}