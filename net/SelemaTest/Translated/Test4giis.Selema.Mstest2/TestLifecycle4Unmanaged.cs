/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Giis.Selema.Framework.Mstest2;
using Giis.Selema.Manager;
using NLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Sharpen;
using Test4giis.Selema.Core;

namespace Test4giis.Selema.Mstest2
{
	[TestClass] public class TestLifecycle4Unmanaged : LifecycleMstest2
	{
		internal static readonly Logger log = LogManager.GetCurrentClassLogger();

		protected internal static LifecycleAsserts lfas = new LifecycleAsserts();

		//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
		protected internal virtual string CurrentName()
		{
			return "TestLifecycle4Unmanaged";
		}

		
          protected internal static SeleniumManager sm;
		public TestLifecycle4Unmanaged() {
        
         sm = LifecycleMstest2.GetManager(sm,Config4test.GetConfig()).SetManagerDelegate(new Config4test()).SetManageNone();

		
		} //public LifecycleMstest2Class cw = new LifecycleMstest2Class(sm);

		
		//public LifecycleMstest2Test tw = new LifecycleMstest2Test(sm, new AfterEachCallback(lfas, log, sm));

		
          [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
		public static new void TearDownClass() {
    
			LifecycleMstest2.TearDownClass();
		}
        public override void RunAfterCallback(string testName, bool success)
        
		{
			new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
		}

		[TestMethod]
		public virtual void TestNoDriver()
		{
			lfas.AssertNow(CurrentName() + ".testNoDriver", sm.CurrentTestName());
			lfas.AssertAfterSetup(sm, false);
			//no debe haber driver activo
			try
			{
				sm.Driver().Url = new Config4test().GetWebUrl();
				//siempre usa la misma pagina
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("should fail");
			}
			catch (Exception e)
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("The Selenium Manager does not have any active WebDriver", e.Message);
			}
			lfas.AssertLast("[ERROR]", "The Selenium Manager does not have any active WebDriver");
		}

		[TestMethod]
		public virtual void TestWithDriver()
		{
			lfas.AssertNow(CurrentName() + ".testWithDriver", sm.CurrentTestName());
			//aunque es unmanaged, uso los metodos de la base para crear y cerrar el driver pero lo mantiene fuera de SeleniumManager
			IWebDriver driver = sm.CreateDriver();
			lfas.AssertAfterSetup(sm, true);
			sm.GetLogger().Info("INSIDE TEST BODY");
			driver.Url = new Config4test().GetWebUrl();
			//siempre usa la misma pagina
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
