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
    /// Support for video recording from a selenoid service: Selenoid produces a video per each driver session,
    /// this service keeps track of the video names, links to videos from the logs and timestamps when failures are produced
    /// </summary>
    public class SelenoidVideoService : AbstractVideoService
    {
        protected override string GetVideoFileNameWithRelativePath(string videoFileName)
        {
            return videoFileName; // video file strucutre is flat
        }

        public override Map<string, object> GetSeleniumOptions(IMediaContext context, string testName)
        {
            string videoFileName = context.GetVideoFileName(testName);
            Map<string, object> opts = new HashMap<string, object>(); // NOSONAR net compatibility
            opts.Put("enableVideo", true);
            opts.Put("videoName", videoFileName);
            return opts;
        }
    }
}