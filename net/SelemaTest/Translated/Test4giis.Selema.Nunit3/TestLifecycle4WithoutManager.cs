/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using NUnit.Framework;
using Sharpen;
using Test4giis.Selema.Core;

namespace Test4giis.Selema.Nunit3
{
	[LifecycleNunit3] public class TestLifecycle4WithoutManager
	{
		protected internal static LogReader reader = new LifecycleAsserts().GetLogReader();

		protected internal static int logSize;

		
		//public LifecycleNunit3Class cw = new LifecycleNunit3Class(null);

		
		//public LifecycleNunit3Test tw = new LifecycleNunit3Test(null);

		[OneTimeSetUp]
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
			catch (SelemaException)
			{
				NUnit.Framework.Assert.AreEqual(logSize, reader.GetLogSize());
			}
		}

		[OneTimeTearDown]
		public static void AfterTearDown()
		{
			NUnit.Framework.Assert.AreEqual(logSize, reader.GetLogSize());
		}
	}
}
