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
    public class SeleniumGridService : SelenoidService
    {
        public override void AddBrowserServiceOptions(Map<string, object> allOptions, IVideoService videoRecorder, IMediaContext mediaVideoContext, string driverScope)
        {

            // Unlike selenoid, browser specific options in grid are not grouped
            allOptions.PutAll(this.GetSeleniumOptions(driverScope));
            if (videoRecorder != null)
                allOptions.PutAll(videoRecorder.GetSeleniumOptions(mediaVideoContext, driverScope));
        }

        public override IVideoService GetVideoRecorder()
        {
            return recordVideo ? new SeleniumGridVideoService() : null;
        }

        public override Map<string, object> GetSeleniumOptions(string sessionName)
        {
            Map<string, object> opts = new HashMap<string, object>(); // NOSONAR net compatibility
            opts.Put("se:vncEnabled", enableVnc); // can't be controlled ad driver instantiation?
            opts.PutAll(specialDriverOptions);
            return opts;
        }
    }
}