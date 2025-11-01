using NLog;
using Giis.Selema.Manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Core
{
    public class AfterEachCallback : IAfterEachCallback
    {
        private LifecycleAsserts steps;
        private Logger log;
        private SeleManager sm;
        public AfterEachCallback(LifecycleAsserts steps, Logger log, SeleManager sm)
        {
            this.steps = steps;
            this.log = log;
            this.sm = sm;
        }

        public virtual void RunAfterCallback(string testName, bool success)
        {
            log.Trace("afterTearDown called"); //steps.assertAfterTeardown(sm, testName, success);
        }
    }
}