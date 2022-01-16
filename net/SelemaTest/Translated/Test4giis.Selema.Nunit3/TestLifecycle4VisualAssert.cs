/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using NLog;
using NUnit.Framework;
using Sharpen;
using Test4giis.Selema.Core;

namespace Test4giis.Selema.Nunit3
{
	/// <summary>Prueba de un visual assert dentro del ciclo de vida (para comparacion manual)</summary>
	[LifecycleNunit3] public class TestLifecycle4VisualAssert : IAfterEachCallback
	{
		internal static readonly Logger log = LogManager.GetCurrentClassLogger();

		protected internal static LifecycleAsserts lfas = new LifecycleAsserts();

		protected internal SeleniumManager sm = new SeleniumManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test());

		
		//public LifecycleNunit3Class cw = new LifecycleNunit3Class(sm);

		
		//public LifecycleNunit3Test tw = new LifecycleNunit3Test(sm, new AfterEachCallback(lfas, log, sm));

		//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
		public virtual void RunAfterCallback(string testName, bool success)
		{
			new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
		}

		[Test]
		public virtual void TestVisualAssertEquals()
		{
			string actual = "uno dos tres\nabc def ghi";
			string expected = "uno tres\nabc XXXX def ghi";
			//Captura la excepcion del assert para comprobar el mensaje
			try
			{
				sm.VisualAssertEquals(expected, actual, "ADDITIONAL MESSAGE");
				NUnit.Framework.Assert.Fail("this assert should fail");
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
