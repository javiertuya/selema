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
    /// Implementation of ICiService used the system is not executed under a supported CI environment
    /// </summary>
    public class LocalService : ICiService
    {
        public virtual bool IsLocal()
        {
            return true;
        }

        public virtual string GetName()
        {
            return "local";
        }

        public virtual string GetJobName()
        {
            return "local-job";
        }

        public virtual string GetJobId()
        {
            return "local-job#0";
        }
    }
}