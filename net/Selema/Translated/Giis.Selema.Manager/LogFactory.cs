using Giis.Selema.Services;
using Giis.Selema.Services.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Manager
{
    /// <summary>
    /// Creates an instance of the selema html logger
    /// </summary>
    public class LogFactory
    {
        //20-jan-2022 removed commented code on factory using standard logger (keep logConfigurator as txt)
        public virtual ISelemaLogger GetLogger(string loggerName, string reportDir, string logFileName)
        {
            return new SelemaLogger(loggerName, reportDir, logFileName);
        }
    }
}