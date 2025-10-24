using Java.Util;
using Giis.Selema.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services.Browser
{
    public class SelenoidBrowserService : AbstractBrowserService
    {
        /// <summary>
        /// Adds the browser service specific capabilities to 'allOptions' map.
        /// In selenoid the capabilities are enclosed under a "selenoid:options" key
        /// </summary>
        public override void AddBrowserServiceOptions(Map<string, object> allOptions, IVideoService videoRecorder, IMediaContext mediaVideoContext, string driverScope)
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
        /// Gets a new instance of the video recorder service associated with this browser service to manage a selenium session
        /// </summary>
        public override IVideoService GetNewVideoRecorder()
        {
            return recordVideo ? new SelenoidVideoService() : null;
        }

        /// <summary>
        /// Gets the capabilities that the IWebDriver must configure to integrate with this service
        /// </summary>
        protected override Map<string, object> GetSeleniumOptions(string sessionName)
        {
            Map<string, object> opts = new HashMap<string, object>(); // NOSONAR net compatibility
            opts.Put("name", sessionName);
            opts.Put("enableVNC", enableVnc);
            opts.PutAll(specialDriverOptions);
            return opts;
        }
    }
}