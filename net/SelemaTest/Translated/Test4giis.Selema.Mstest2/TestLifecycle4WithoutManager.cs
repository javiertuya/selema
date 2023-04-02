/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Framework.Mstest2;
using Giis.Selema.Manager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpen;
using Test4giis.Selema.Core;

namespace Test4giis.Selema.Mstest2
{
	[TestClass] public class TestLifecycle4WithoutManager
	{
		protected internal static LogReader reader = new LifecycleAsserts().GetLogReader();

		protected internal static int logSize;

		
		//public LifecycleMstest2Class cw = new LifecycleMstest2Class(null);

		
		//public LifecycleMstest2Test tw = new LifecycleMstest2Test(null);

		[ClassInitialize]
		public static void SetUpClass(TestContext TestContext)
		{
			logSize = reader.GetLogSize();
		}

		[TestMethod]
		public virtual void TestFailedTestNoWriteLogs()
		{
			//a failed test should not raise exception nor write logs
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
