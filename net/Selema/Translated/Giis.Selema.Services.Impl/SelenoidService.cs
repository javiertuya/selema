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
    public class SelenoidService : IBrowserService
    {
        private bool recordVideo = false; //si true habilita el video recording
        private bool enableVnc = false; //si true habilita VNC par ver las sesiones desde selenoid-ui
        private Map<string, object> specialDriverOptions = new HashMap<string, object>(); // NOSONAR net compatibility
        /// <summary>
        /// Activates the video recording, provided that the Selenoid server is configured for video recording
        /// </summary>
        public virtual IBrowserService SetVideo()
        {
            this.recordVideo = true;
            return this;
        }

        /// <summary>
        /// ctivates the VNC capabilities to be able to watch the test execution in real time (e.g. using selenoid-ui)
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
        /// Gets the video recorder service associated with this browser service
        /// </summary>
        public virtual IVideoService GetVideoRecorder()
        {
            return recordVideo ? new SelenoidVideoService() : null;
        }

        /// <summary>
        /// Gets the capabilities that the IWebDriver must configure to integrate with this service
        /// </summary>
        public virtual Map<string, object> GetSeleniumOptions(string sessionName)
        {
            Map<string, object> opts = new HashMap<string, object>(); // NOSONAR net compatibility
            opts.Put("name", sessionName);
            opts.Put("enableVNC", enableVnc);
            opts.PutAll(specialDriverOptions);
            return opts;
        }
    }
}