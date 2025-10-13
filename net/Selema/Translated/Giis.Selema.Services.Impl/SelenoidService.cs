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
        protected bool recordVideo = false; //si true habilita el video recording
        protected bool enableVnc = false; //si true habilita VNC par ver las sesiones desde selenoid-ui
        protected Map<string, object> specialDriverOptions = new HashMap<string, object>(); // NOSONAR net compatibility
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
        /// Adds the browser service specific capabilities to 'allOptions' map.
        /// In selenoid the capabilities are enclosed under a "selenoid:options" key
        /// </summary>
        public virtual void AddBrowserServiceOptions(Map<string, object> allOptions, IVideoService videoRecorder, IMediaContext mediaVideoContext, string driverScope)
        {

            //Although browser service and video recorder are handled independently, in the case of Selenoid:
            //-using Selenium 4.1.0 on .NET, options are not passed to the driver
            //-it is required to pass all selenoid related options as IWebDriver protocol extension as a pair "selenoid:options", <map with all options>
            Map<string, object> selenoidOptions = new HashMap<string, object>(); // NOSONAR net compatibility
            selenoidOptions.PutAll(this.GetSeleniumOptions(driverScope));
            if (videoRecorder != null)
                selenoidOptions.PutAll(videoRecorder.GetSeleniumOptions(mediaVideoContext, driverScope));
            allOptions.Put("selenoid:options", selenoidOptions);
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