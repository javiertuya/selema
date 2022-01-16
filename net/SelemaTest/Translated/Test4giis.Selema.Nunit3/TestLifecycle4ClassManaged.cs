/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using NLog;
using NUnit.Framework;
using OpenQA.Selenium;
using Sharpen;
using Test4giis.Selema.Core;

namespace Test4giis.Selema.Nunit3
{
	[LifecycleNunit3] public class TestLifecycle4ClassManaged : IAfterEachCallback
	{
		internal static readonly Logger log = LogManager.GetCurrentClassLogger();

		protected internal static LifecycleAsserts lfas = new LifecycleAsserts();

		//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
		protected internal virtual string CurrentName()
		{
			return "TestLifecycle4ClassManaged";
		}

		protected internal static int thisTestCount = 0;

		protected internal SeleniumManager sm = new SeleniumManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test()).SetManageAtClass().SetDriverDelegate(new DriverConfigMaximize());

		
		//public LifecycleNunit3Class cw = new LifecycleNunit3Class(sm);

		
		//public LifecycleNunit3Test tw = new LifecycleNunit3Test(sm, new AfterEachCallback(lfas, log, sm));

		//this sm includes a configuration of the driver (check that driver runs maximized)
		public virtual void RunAfterCallback(string testName, bool success)
		{
			new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
		}

		protected internal virtual void LaunchPage()
		{
			sm.Driver().Url = new Config4test().GetWebUrl();
			//siempre usa la misma pagina
			sm.Watermark();
		}

		[Test]
		public virtual void TestFailMethod()
		{
			lfas.AssertNow(CurrentName() + ".testFailMethod", sm.CurrentTestName());
			lfas.AssertAfterSetup(sm, false, thisTestCount == 0);
			thisTestCount++;
			LaunchPage();
			sm.OnFailure(CurrentName(), sm.CurrentTestName());
			lfas.AssertAfterFail(sm);
		}

		[Test]
		public virtual void TestPassMethod()
		{
			lfas.AssertNow(CurrentName() + ".testPassMethod", sm.CurrentTestName());
			lfas.AssertAfterSetup(sm, false, thisTestCount == 0);
			thisTestCount++;
			LaunchPage();
			sm.GetLogger().Info("INSIDE TEST BODY");
			NUnit.Framework.Assert.AreEqual("result", sm.Driver().FindElement(By.Id("spanAlert")).Text);
			lfas.AssertAfterPass();
		}
	}
}
