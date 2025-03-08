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
    /// Information about the jenkins environment and job identification
    /// </summary>
    public class JenkinsService : ICiService
    {
        public virtual bool IsLocal()
        {
            return false;
        }

        public virtual string GetName()
        {
            return "Jenkins";
        }

        /// <summary>
        /// Returns the job name as provided by the Jenkins environment (in a multibranch pipeline, it includes the branch name)
        /// </summary>
        public virtual string GetJobName()
        {
            return JavaCs.GetEnvironmentVariable("JOB_NAME");
        }

        /// <summary>
        /// Returns the job execution unique identifier as provided by the Jenkins environment (includes the job name and build number)
        /// </summary>
        public virtual string GetJobId()
        {
            return GetJobName() + "#" + JavaCs.GetEnvironmentVariable("BUILD_NUMBER");
        }
    }
}