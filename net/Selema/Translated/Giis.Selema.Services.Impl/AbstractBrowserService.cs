using Java.Util;
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
    /// Base class for all remote browser services
    /// </summary>
    public abstract class AbstractBrowserService : IBrowserService
    {
        protected bool recordVideo = false; // si true habilita el video recording
        protected bool enableVnc = false; // si true habilita VNC par ver las sesiones desde selenoid-ui
        protected Map<string, object> specialDriverOptions = new HashMap<string, object>(); // NOSONAR net compatibility
        /// <summary>
        /// Activates the video recording, provided that the browser server is configured for video recording
        /// </summary>
        public virtual IBrowserService SetVideo()
        {
            this.recordVideo = true;
            return this;
        }

        /// <summary>
        /// Activates the VNC capabilities to be able to watch the test execution in real time (e.g. using selenoid-ui)
        /// </summary>
        public virtual IBrowserService SetVnc()
        {
            enableVnc = true;
            return this;
        }

        /// <summary>
        /// Adds a special capability other than the predefined video and vnc
        /// </summary>
        public virtual IBrowserService SetCapability(string key, object value)
        {
            specialDriverOptions.Put(key, value);
            return this;
        }

        /// <summary>
        /// Adds the browser service specific capabilities to 'allOptions' map.
        /// </summary>
        public virtual void AddBrowserServiceOptions(Map<string, object> allOptions, IVideoService videoRecorder, IMediaContext mediaVideoContext, string driverScope)
        {

            // Unlike selenoid, by default, browser specific options in grid are not grouped
            allOptions.PutAll(this.GetSeleniumOptions(driverScope));
            if (videoRecorder != null)
                allOptions.PutAll(videoRecorder.GetSeleniumOptions(mediaVideoContext, driverScope));
        }

        /// <summary>
        /// Gets a new instance of the video recorder service associated with this browser service
        /// </summary>
        public abstract IVideoService GetVideoRecorder();
        /// <summary>
        /// Gets the capabilities that the IWebDriver must configure to integrate with this service
        /// </summary>
        public virtual Map<string, object> GetSeleniumOptions(string sessionName)
        {
            Map<string, object> opts = new HashMap<string, object>(); // NOSONAR net compatibility

            // By default, no additional options are added
            opts.PutAll(specialDriverOptions);
            return opts;
        }
    }
}