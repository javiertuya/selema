using Giis.Portable.Util;
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
    /// Manages the filenames of media files produced durint testing (screenshots, videos, diff files)
    /// </summary>
    public class MediaContext : IMediaContext
    {
        private string reportFolder;
        private string qualifier;
        private int instanceCount; //uniquely dentifies each SeleManager instance
        private int sessionCount; //uniquely identifies each new driver session created by this instance
        private int inSessionCount; //given a session, gets unique identifiers, if needed
        public MediaContext(string reportFolder, string qualifier, int instanceCount, int sessionCount)
        {
            this.reportFolder = reportFolder;
            this.qualifier = qualifier;
            this.instanceCount = instanceCount;
            this.sessionCount = sessionCount;
            this.inSessionCount = 0;
        }

        public virtual string GetReportFolder()
        {
            return reportFolder;
        }

        public virtual string GetScreenshotFileName(string testName)
        {
            return "screen" + qualifier + "-" + GetMediaFileName(testName, true) + ".png";
        }

        public virtual string GetVideoFileName(string testName)
        {
            return "video" + qualifier + "-" + GetMediaFileName(testName, false) + ".mp4";
        }

        public virtual string GetDiffFileName(string testName)
        {
            return "diff" + qualifier + "-" + GetMediaFileName(testName, true) + ".html";
        }

        private string GetMediaFileName(string testName, bool includeInSessionCount)
        {
            string prefix = DoubleDigit(instanceCount) + "-" + DoubleDigit(sessionCount) + "-";
            if (includeInSessionCount)
            {
                inSessionCount++;
                prefix = prefix + DoubleDigit(inSessionCount) + "-";
            }

            return prefix + ReprocessFileName(testName);
        }

        private string DoubleDigit(long value)
        {
            return (value < 10 ? "0" : "") + (value);
        }

        /// <summary>
        /// Removes all no alphanumerich characters to representa filename (without extension)
        /// </summary>
        public static string ReprocessFileName(string name)
        {
            if (name == null || "".Equals(name))
                return "noname";

            //sharpen in windows replaces non ascii (like enye) correctly
            //but in linux container replaces them to ??, make this first replacement to - in order to maintain compatibility between platforms
            string replaced = name.Replace("??", "-");
            replaced = JavaCs.ReplaceRegex(replaced, "[^A-Za-z0-9]", "-");
            return replaced.Length > 100 ? JavaCs.Substring(replaced, 0, 100) : replaced;
        }
    }
}