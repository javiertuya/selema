/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Framework.Mstest2;
using Giis.Selema.Manager;
using NLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Sharpen;
using Test4giis.Selema.Core;

namespace Test4giis.Selema.Mstest2
{
	[TestClass] public class TestLifecycle4ClassManaged : LifecycleMstest2
	{
		internal static readonly Logger log = LogManager.GetCurrentClassLogger();

		protected internal static LifecycleAsserts lfas = new LifecycleAsserts();

		//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
		protected internal virtual string CurrentName()
		{
			return "TestLifecycle4ClassManaged";
		}

		protected internal static int thisTestCount = 0;

		
          protected internal static SeleniumManager sm;
		public TestLifecycle4ClassManaged() {
        
         sm = LifecycleMstest2.GetManager(sm,Config4test.GetConfig()).SetManagerDelegate(new Config4test()).SetManageAtClass().SetDriverDelegate(new DriverConfigMaximize());

		
		} //public LifecycleMstest2Class cw = new LifecycleMstest2Class(sm);

		
		//public LifecycleMstest2Test tw = new LifecycleMstest2Test(sm, new AfterEachCallback(lfas, log, sm));

		//this sm includes a configuration of the driver (check that driver runs maximized)
		
          [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
		public static new void TearDownClass() {
    
			LifecycleMstest2.TearDownClass();
		}
        public override void RunAfterCallback(string testName, bool success)
        
		{
			new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
		}

		protected internal virtual void LaunchPage()
		{
			sm.Driver().Url = new Config4test().GetWebUrl();
			//siempre usa la misma pagina
			sm.Watermark();
		}

		[TestMethod]
		public virtual void TestFailMethod()
		{
			lfas.AssertNow(CurrentName() + ".testFailMethod", sm.CurrentTestName());
			lfas.AssertAfterSetup(sm, false, thisTestCount == 0);
			thisTestCount++;
			LaunchPage();
			sm.OnFailure(CurrentName(), sm.CurrentTestName());
			lfas.AssertAfterFail(sm);
		}

		[TestMethod]
		public virtual void TestPassMethod()
		{
			lfas.AssertNow(CurrentName() + ".testPassMethod", sm.CurrentTestName());
			lfas.AssertAfterSetup(sm, false, thisTestCount == 0);
			thisTestCount++;
			LaunchPage();
			sm.GetLogger().Info("INSIDE TEST BODY");
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("result", sm.Driver().FindElement(By.Id("spanAlert")).Text);
			lfas.AssertAfterPass();
		}
	}
}
