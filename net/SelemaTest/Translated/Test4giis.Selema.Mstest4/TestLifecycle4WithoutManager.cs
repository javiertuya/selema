using Microsoft.VisualStudio.TestTools.UnitTesting;
using Giis.Selema.Framework.Mstest4;
using Giis.Selema.Manager;
using Test4giis.Selema.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Mstest4
{
    [TestClass] public class TestLifecycle4WithoutManager
    {
        protected static LogReader reader = new LifecycleAsserts().GetLogReader();
        protected static int logSize;
        //public static LifecycleJunit4Class cw = new LifecycleJunit4Class(null);
        //public LifecycleJunit4Test tw = new LifecycleJunit4Test(null);
        [ClassInitialize]
        public static void SetUpClass(TestContext TestContext)
        {
            logSize = reader.GetLogSize();
        }

        [TestMethod]
        public virtual void TestFailedTestNoWriteLogs()
        {

            //a failed test should not raise exception nor write logs
            //do not use assertThrows or similar for compatibility with net conversion and different frameworks
            try
            {
                throw new SelemaException("Exception to be catched");
            }
            catch (SelemaException)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(logSize, reader.GetLogSize());
            }
        }

        [ClassCleanup]
        public static void AfterTearDown()
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(logSize, reader.GetLogSize());
        }
    }
}