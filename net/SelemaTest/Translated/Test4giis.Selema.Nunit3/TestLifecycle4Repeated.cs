using NUnit.Framework;
using NLog;
using Giis.Selema.Framework.Nunit3;

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

namespace Test4giis.Selema.Nunit3
{
    [LifecycleNunit3] public class TestLifecycle4Repeated : IAfterEachCallback
    {
        static readonly Logger log = LogManager.GetCurrentClassLogger(); //TestLifecycle4Repeated));
        protected static LifecycleAsserts lfas = new LifecycleAsserts();
        protected internal SeleManager sm = new SeleManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test()).Add(new WatermarkService()).SetMaximize(true);
        //public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
        //public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm, new AfterEachCallback(lfas, log, sm));
        //public RepeatedTestRule repeatRule = new RepeatedTestRule(sm, new AfterEachCallback(lfas, log, sm));
        public virtual void RunAfterCallback(string testName, bool success)
        {
            new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp()
        {
            DriverUtil.GetUrl(sm.Driver, new Config4test().GetWebUrl());
            sm.Watermark();
        }

        private static int repetitions = 0;
        [Test]
        [Retry(3)] public virtual void TestRepeated()
        {
            lfas.AssertAfterSetup(sm, true);
            repetitions++;
            if (repetitions < 3)
                NUnit.Framework.Legacy.ClassicAssert.Fail("simulated failure");
            else if (new Config4test().GetManualCheckEnabled())
                NUnit.Framework.Legacy.ClassicAssert.Fail("Manual Check: salida estandar del test muestra nombres de test y log de grabacion video y screenshot");
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(true);
        }
    }
}