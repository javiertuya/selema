using Java.Util;
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
    public class DynamicGridVideoService : AbstractVideoService
    {
        protected override string GetVideoFileNameWithRelativePath(IMediaContext context, string testName)
        {
            string videoFileName = base.GetVideoFileNameWithRelativePath(context, testName);

            // I don't know why, but the recorded video names have a suffix mp4 before the .mp4 file name extension
            videoFileName = JavaCs.Substring(videoFileName, 0, videoFileName.Length - 4) + "mp4.mp4";

            // Grid places each video under a different folder
            return "".Equals(seleniumSessionId) ? seleniumSessionId : seleniumSessionId + "/" + videoFileName;
        }

        public override Map<string, object> GetSeleniumOptions(IMediaContext context, string testName)
        {
            string videoFileName = context.GetVideoFileName(testName);
            Map<string, object> opts = new HashMap<string, object>(); // NOSONAR net compatibility
            opts.Put("se:recordVideo", true);
            opts.Put("se:name", videoFileName);
            return opts;
        }
    }
}