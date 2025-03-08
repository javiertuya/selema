using Giis.Portable.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Manager
{
    /// <summary>
    /// Static configuration parameters at the instantiation of a SeleManager (location, name, etc)
    /// </summary>
    public class SelemaConfig
    {
        private static readonly string DEFAULT_NAME = "selema";
        //Path to the project root (depends on the platform)
        private string projectRoot = Parameters.DefaultProjectRoot;
        //Path to the selema reports folder relative to projectRoot (depends on the platform)
        private string reportSubdir = Parameters.DefaultReportSubdir;
        private string logName = DEFAULT_NAME;
        /// <summary>
        /// Gets the project root currently configured
        /// </summary>
        public virtual string GetProjectRoot()
        {
            return projectRoot;
        }

        /// <summary>
        /// Changes the location of the project root;
        /// Default is current folder on Java and four levels up on NET
        /// (this is the solution folder provided that the test project is located just below the solution folder)
        /// </summary>
        public virtual SelemaConfig SetProjectRoot(string root)
        {
            projectRoot = root;
            return this;
        }

        /// <summary>
        /// Gets the selema report folder currently configured
        /// </summary>
        public virtual string GetReportSubdir()
        {
            return reportSubdir;
        }

        /// <summary>
        /// Changes the name of the report folder (relative to the project root). Default is `target` (on Java) and `reports` (on .NET).
        /// </summary>
        public virtual SelemaConfig SetReportSubdir(string subdir)
        {
            reportSubdir = subdir;
            return this;
        }

        public virtual string GetReportDir()
        {
            return FileUtil.GetPath(projectRoot, reportSubdir);
        }

        public virtual string GetName()
        {
            return logName;
        }

        /// <summary>
        /// Sets the name of the SeleManager (default is selema), useful when logs from different instances must be kept separated
        /// </summary>
        public virtual SelemaConfig SetName(string name)
        {
            logName = name;
            return this;
        }

        public virtual string GetQualifier()
        {
            return logName.Equals(DEFAULT_NAME) ? "" : "-" + logName;
        }

        public virtual string GetLogName()
        {
            return "selema-log" + GetQualifier() + ".html";
        }
    }
}