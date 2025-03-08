using NUnit.Framework;
using NLog;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using Giis.Selema.Portable.Selenium;
using Test4giis.Selema.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Nunit3
{
    [LifecycleNunit3] public class TestLifecycle4ClassManaged : IAfterEachCallback
    {
        //interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
        static readonly Logger log = LogManager.GetCurrentClassLogger(); //TestLifecycle4ClassManaged));
        protected static LifecycleAsserts lfas = new LifecycleAsserts();
        protected virtual string CurrentName()
        {
            return "TestLifecycle4ClassManaged";
        }

        protected static int thisTestCount = 0;
        //this sm includes a configuration of the driver (check that driver runs maximized)
        protected internal SeleManager sm = new SeleManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test()).SetManageAtClass().SetDriverDelegate(new DriverConfigMaximize());
        //public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
        //public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm, new AfterEachCallback(lfas, log, sm));
        public virtual void RunAfterCallback(string testName, bool success)
        {
            new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
        }

        protected virtual void LaunchPage()
        {
            DriverUtil.GetUrl(sm.Driver, new Config4test().GetWebUrl()); //siempre usa la misma pagina
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
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("result", sm.Driver.FindElement(OpenQA.Selenium.By.Id("spanAlert")).Text);
            lfas.AssertAfterPass();
        }
    }
}