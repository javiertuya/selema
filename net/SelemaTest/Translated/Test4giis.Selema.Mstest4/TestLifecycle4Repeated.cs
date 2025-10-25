using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using Giis.Selema.Framework.Mstest4;

using Giis.Selema.Manager;
using Giis.Selema.Portable.Selenium;
using Giis.Selema.Services.Impl;
using Test4giis.Selema.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Mstest4
{
    [TestClass] public class TestLifecycle4Repeated : LifecycleMstest4
    {
        static readonly Logger log = LogManager.GetCurrentClassLogger(); //TestLifecycle4Repeated));
        protected static LifecycleAsserts lfas = new LifecycleAsserts();
        
          protected internal static SeleManager sm;
		      public TestLifecycle4Repeated() {
          
         sm = LifecycleMstest4.GetManager(sm,Config4test.GetConfig()).SetManagerDelegate(new Config4test()).Add(new WatermarkService()).SetMaximize(true);
        //public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
        //public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm, new AfterEachCallback(lfas, log, sm));
        //public RepeatedTestRule repeatRule = new RepeatedTestRule(sm, new AfterEachCallback(lfas, log, sm));
        
          } [ClassCleanup()]
#pragma warning disable MSTEST0036 // Do not use shadowing
		      public static new void TearDownClass() { 
            LifecycleMstest4.TearDownClass(); 
          }
#pragma warning restore MSTEST0036 // Do not use shadowing
          public override void RunAfterCallback(string testName, bool success)
        
        {
            new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
        }

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp(); DriverUtil.GetUrl(sm.Driver, new Config4test().GetWebUrl());
            sm.Watermark();
        }

        private static int repetitions = 0;
        
        [RetryTestMethod(3)] public virtual void TestRepeated()
        {
            lfas.AssertAfterSetup(sm, true);
            repetitions++;
            if (repetitions < 3)
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("simulated failure");
            else if (new Config4test().GetManualCheckEnabled())
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("Manual Check: salida estandar del test muestra nombres de test y log de grabacion video y screenshot");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(true);
        }
    }
}