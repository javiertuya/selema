/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using NLog;
using NUnit.Framework;
using OpenQA.Selenium;
using Sharpen;
using Test4giis.Selema.Core;

namespace Test4giis.Selema.Nunit3
{
	[LifecycleNunit3] public class TestLifecycle4Unmanaged : IAfterEachCallback
	{
		internal static readonly Logger log = LogManager.GetCurrentClassLogger();

		protected internal static LifecycleAsserts lfas = new LifecycleAsserts();

		//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
		protected internal virtual string CurrentName()
		{
			return "TestLifecycle4Unmanaged";
		}

		protected internal SeleManager sm = new SeleManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test()).SetManageNone();

		
		//public LifecycleNunit3Class cw = new LifecycleNunit3Class(sm);

		
		//public LifecycleNunit3Test tw = new LifecycleNunit3Test(sm, new AfterEachCallback(lfas, log, sm));

		public virtual void RunAfterCallback(string testName, bool success)
		{
			new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
		}

		[Test]
		public virtual void TestNoDriver()
		{
			lfas.AssertNow(CurrentName() + ".testNoDriver", sm.CurrentTestName());
			lfas.AssertAfterSetup(sm, false);
			//should not have an active driver, accesing to it throws exception
			NUnit.Framework.Legacy.ClassicAssert.IsFalse(sm.HasDriver());
			try
			{
				sm.Driver.Url = new Config4test().GetWebUrl();
				//siempre usa la misma pagina
				NUnit.Framework.Legacy.ClassicAssert.Fail("should fail");
			}
			catch (Exception e)
			{
				NUnit.Framework.Legacy.ClassicAssert.AreEqual("The Selenium Manager does not have any active WebDriver", e.Message);
			}
			lfas.AssertLast("[ERROR]", "The Selenium Manager does not have any active WebDriver");
		}

		[Test]
		public virtual void TestWithDriver()
		{
			lfas.AssertNow(CurrentName() + ".testWithDriver", sm.CurrentTestName());
			//even if it is unmanaged, I can create a driver that is bound to the manager
			IWebDriver driver = sm.CreateDriver();
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(sm.HasDriver());
			lfas.AssertAfterSetup(sm, true);
			sm.GetLogger().Info("INSIDE TEST BODY");
			driver.Url = new Config4test().GetWebUrl();
			lfas.AssertAfterPass();
			sm.QuitDriver(driver);
			sm.QuitDriver(driver);
			//ensures can be called multiple times
			//despues de cerrar el driver se guardan los videos, estas acciones se deben comprobar antes del teardown para driver remoto
			if (!string.Empty.Equals(sm.GetDriverUrl()))
			{
				lfas.GetLogReader().AssertBegin();
				lfas.GetLogReader().AssertContains("Saving video", CurrentName() + "-testWithDriver");
				lfas.GetLogReader().AssertContains("Remote session ending");
				lfas.GetLogReader().AssertEnd();
			}
		}
	}
}
