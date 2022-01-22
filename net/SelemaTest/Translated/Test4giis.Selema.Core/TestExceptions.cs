/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using Giis.Selema.Portable;
using Giis.Selema.Services;
using Giis.Selema.Services.Impl;
using NLog;
using NUnit.Framework;
using OpenQA.Selenium;
using Sharpen;

namespace Test4giis.Selema.Core
{
	/// <summary>
	/// Checks exceptional situations: Some of them do not raise exceptions, but write in the selema log,
	/// others should raise exception to the user.
	/// </summary>
	/// <remarks>
	/// Checks exceptional situations: Some of them do not raise exceptions, but write in the selema log,
	/// others should raise exception to the user.
	/// Note that exceptions are tested using try catch to allow automatic conversion to nunit
	/// </remarks>
	[LifecycleNunit3] public class TestExceptions : IAfterEachCallback
	{
		internal static readonly Logger log = LogManager.GetCurrentClassLogger();

		protected internal static LifecycleAsserts lfas = new LifecycleAsserts();

		//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
		protected internal virtual string CurrentName()
		{
			return "TestExceptions";
		}

		protected internal static int thisTestCount = 0;

		protected internal SeleniumManager sm = new SeleniumManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test(false)).SetManageAtClass().SetDriverDelegate(new DriverConfigMaximize());

		
		//public LifecycleNunit3Class cw = new LifecycleNunit3Class(sm);

		
		//public LifecycleNunit3Test tw = new LifecycleNunit3Test(sm);

		protected internal static IWebDriver saveDriver;

		//this sm includes a configuration of the driver (check that driver runs maximized)
		public virtual void RunAfterCallback(string testName, bool success)
		{
			new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
		}

		//Dirty: Driver is managed at class level for performance
		//but some tests manipulate the driver, save an restore for each test
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			saveDriver = sm.Driver;
			lfas.AssertAfterSetup(sm, false, thisTestCount == 0);
			//ensures correct setup
			LaunchPage();
		}

		[NUnit.Framework.TearDown]
		public virtual void TearDown()
		{
			thisTestCount++;
			//only 0 in first test
			sm.ReplaceDriver(saveDriver);
		}

		protected internal virtual void LaunchPage()
		{
			sm.Driver.Url = new Config4test().GetWebUrl();
		}

		//siempre usa la misma pagina
		[Test]
		public virtual void TestManagerWithoutConfig()
		{
			try
			{
				new SeleniumManager(null);
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch (SelemaException e)
			{
				NUnit.Framework.Assert.AreEqual("SeleniumManager instance requires an instance of SelemaConfig", e.Message);
			}
		}

		//uses a different report subdir to do not include wrong named files/folders that cause error when published as artifacts
		[Test]
		public virtual void TestManagerWrongName()
		{
			try
			{
				new SeleniumManager(new SelemaConfig().SetReportSubdir("dat/tmp").SetName("ab?cd"));
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch (Exception)
			{
			}
		}

		[Test]
		public virtual void TestManagerWrongReportSubdir()
		{
			try
			{
				new SeleniumManager(new SelemaConfig().SetReportSubdir("dat/tmp/ab?cd"));
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch (Exception)
			{
			}
		}

		[Test]
		public virtual void TestManagerWrongProjectRoot()
		{
			try
			{
				new SeleniumManager(new SelemaConfig().SetProjectRoot("dat/tmp/ab?cd"));
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch (Exception)
			{
			}
		}

		[Test]
		public virtual void TestScreenshotExceptionByDriver()
		{
			//first screenshot pass
			sm.Screenshot("forced-screenshot");
			lfas.AssertLast("[INFO]", "Taking screenshot", "forced-screenshot.png");
			//forces exception by passing a null driver
			IMediaContext context = new MediaContext(sm.GetConfig().GetReportDir(), sm.GetConfig().GetQualifier(), 99, 99);
			sm.GetScreenshotService().TakeScreenshot(null, context, "TestExceptions.testScreenshotInternalException");
			lfas.AssertLast("[ERROR]", "Can't take screenshot or write the content to file", "TestExceptions-testScreenshotInternalException.png");
		}

		[Test]
		public virtual void TestScreenshotExceptionWriting()
		{
			//forces exception writing by pasing an invalid report dir
			IMediaContext context = new MediaContext(sm.GetConfig().GetProjectRoot() + "/dat/tmp/ab?cd", sm.GetConfig().GetQualifier(), 99, 99);
			sm.GetScreenshotService().TakeScreenshot(sm.Driver, context, "TestExceptions.testScreenshotInternalException");
			lfas.AssertLast("[ERROR]", "Can't take screenshot or write the content to file", "TestExceptions-testScreenshotInternalException.png");
		}

		[Test]
		public virtual void TestVisualAssertException()
		{
			sm.VisualAssertEquals("ab cd", "ab cd");
			//first assert pass
			try
			{
				sm.VisualAssertEquals("ab cd", "ab xy cd");
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch
			{
			}
			lfas.AssertLast("[WARN]", "Visual Assert differences", "TestExceptions-testVisualAssertException.html");
		}

		[Test]
		public virtual void TestWatermarkException()
		{
			try
			{
				sm.Watermark();
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch
			{
			}
			lfas.AssertLast("[ERROR]", "Watermark service is not attached to this Selenium Manager");
		}
	}
}
