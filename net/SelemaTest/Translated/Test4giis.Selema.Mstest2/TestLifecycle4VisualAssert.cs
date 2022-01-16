/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Giis.Selema.Framework.Mstest2;
using Giis.Selema.Manager;
using NLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpen;
using Test4giis.Selema.Core;

namespace Test4giis.Selema.Mstest2
{
	/// <summary>Prueba de un visual assert dentro del ciclo de vida (para comparacion manual)</summary>
	[TestClass] public class TestLifecycle4VisualAssert : LifecycleMstest2
	{
		internal static readonly Logger log = LogManager.GetCurrentClassLogger();

		protected internal static LifecycleAsserts lfas = new LifecycleAsserts();

		
          protected internal static SeleniumManager sm;
		public TestLifecycle4VisualAssert() {
        
         sm = LifecycleMstest2.GetManager(sm,Config4test.GetConfig()).SetManagerDelegate(new Config4test());

		
		} //public LifecycleMstest2Class cw = new LifecycleMstest2Class(sm);

		
		//public LifecycleMstest2Test tw = new LifecycleMstest2Test(sm, new AfterEachCallback(lfas, log, sm));

		//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
		
          [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
		public static new void TearDownClass() {
    
			LifecycleMstest2.TearDownClass();
		}
        public override void RunAfterCallback(string testName, bool success)
        
		{
			new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
		}

		[TestMethod]
		public virtual void TestVisualAssertEquals()
		{
			string actual = "uno dos tres\nabc def ghi";
			string expected = "uno tres\nabc XXXX def ghi";
			//Captura la excepcion del assert para comprobar el mensaje
			try
			{
				sm.VisualAssertEquals(expected, actual, "ADDITIONAL MESSAGE");
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("this assert should fail");
			}
			catch (Exception)
			{
				LogReader reader = lfas.GetLogReader();
				reader.AssertBegin();
				reader.AssertContains("Visual Assert differences", "testVisualAssertEquals");
				reader.AssertEnd();
			}
		}
	}
}
