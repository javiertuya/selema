using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Manager
{
    /// <summary>
    /// Strategies for selecting the driver version to download
    /// </summary>
    public class DriverVersion
    {
        public static readonly string MATCH_BROWSER = "match"; // try match with browser version
        public static readonly string LATEST_AVAILABLE = "latest"; // use the latest available version
        public static readonly string SELENIUM_MANAGER = "selenium"; // use the default SeleniumManager
        public static readonly string DEFAULT = MATCH_BROWSER;
        private DriverVersion()
        {
            throw new InvalidOperationException("Utility class");
        }
    }
}