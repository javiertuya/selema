using NUnit.Framework;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using Test4giis.Selema.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Nunit3
{
    [LifecycleNunit3] public class TestLifecycle4WithoutManager
    {
        protected static LogReader reader = new LifecycleAsserts().GetLogReader();
        protected static int logSize;
        //public static LifecycleJunit4Class cw = new LifecycleJunit4Class(null);
        //public LifecycleJunit4Test tw = new LifecycleJunit4Test(null);
        [NUnit.Framework.OneTimeSetUp]
        public static void SetUpClass()
        {
            logSize = reader.GetLogSize();
        }

        [Test]
        public virtual void TestFailedTestNoWriteLogs()
        {

            //a failed test should not raise exception nor write logs
            try
            {
                throw new SelemaException("Exception to be catched");
            }
            catch (SelemaException e)
            {
                NUnit.Framework.Legacy.ClassicAssert.AreEqual(logSize, reader.GetLogSize());
            }
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterTearDown()
        {
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(logSize, reader.GetLogSize());
        }
    }
}