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

            // Grid places each video under a folder named as the session id, inside the folder mounted at /opt/selenium/assets.
            // To make the link resolve from the selema log, this folder must be mounted at the selema report folder
            return "".Equals(seleniumSessionId) ? seleniumSessionId : seleniumSessionId + "/" + videoFileName;
        }

        public override Map<string, object> GetSeleniumOptions(IMediaContext context, string testName)
        {

            // Grid names the video as the se:name capability plus the .mp4 extension, after removing the characters
            // other than letters, digits, hyphens and underscores (the dot of the extension would be removed)
            string videoFileName = context.GetVideoFileName(testName);
            string videoName = JavaCs.Substring(videoFileName, 0, videoFileName.Length - ".mp4".Length);
            Map<string, object> opts = new HashMap<string, object>(); // NOSONAR net compatibility
            opts.Put("se:recordVideo", true);
            opts.Put("se:name", videoName);
            return opts;
        }
    }
}