/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Manager;
using NLog;
using Sharpen;

namespace Test4giis.Selema.Core
{
	public class AfterEachCallback : IAfterEachCallback
	{
		private LifecycleAsserts steps;

		private Logger log;

		private SeleniumManager sm;

		public AfterEachCallback(LifecycleAsserts steps, Logger log, SeleniumManager sm)
		{
			this.steps = steps;
			this.log = log;
			this.sm = sm;
		}

		public virtual void RunAfterCallback(string testName, bool success)
		{
			log.Trace("afterTearDown called");
			steps.AssertAfterTeardown(sm, testName, success);
		}
	}
}
