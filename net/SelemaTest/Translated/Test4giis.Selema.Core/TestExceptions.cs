/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
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

		protected internal SeleManager sm = new SeleManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test(false)).SetManageAtClass().SetDriverDelegate(new DriverConfigMaximize());

		
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
			sm.Add((WatermarkService)null);
			// some tests add this service, this removes it
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
				new SeleManager(null);
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch (SelemaException e)
			{
				NUnit.Framework.Assert.AreEqual("SeleManager instance requires an instance of SelemaConfig", e.Message);
			}
		}

		//uses a different report subdir to do not include wrong named files/folders that cause error when published as artifacts
		[Test]
		public virtual void TestManagerWrongName()
		{
			try
			{
				new SeleManager(new SelemaConfig().SetReportSubdir("dat/tmp").SetName("ab\x0"));
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
				new SeleManager(new SelemaConfig().SetReportSubdir("dat/tmp/ab\x0"));
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
				new SeleManager(new SelemaConfig().SetProjectRoot("dat/tmp/ab\x0"));
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
			IMediaContext context = new MediaContext(sm.GetConfig().GetProjectRoot() + "/dat/tmp/ab\x0", sm.GetConfig().GetQualifier(), 99, 99);
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
			//with message
			try
			{
				sm.VisualAssertEquals("ef gh", "ef zt gh", "va message");
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch
			{
			}
			lfas.AssertLast("[WARN]", "Visual Assert differences", "TestExceptions-testVisualAssertException.html", "va message");
		}

		[Test]
		public virtual void TestSoftAssertException()
		{
			sm.SoftAssertClear();
			sm.SoftAssertEquals("ab cd", "ab cd");
			//first assert pass
			sm.SoftAssertEquals("ab cd", "ab xy cd");
			sm.SoftAssertEquals("ef gh", "ef zt gh", "sva message");
			try
			{
				sm.SoftAssertAll();
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch
			{
			}
			lfas.AssertLast(0, "[WARN]", "Soft Visual Assert differences (Failure 2)", "TestExceptions-testSoftAssertException.html", "sva message");
			lfas.AssertLast(1, "[WARN]", "Soft Visual Assert differences (Failure 1)", "TestExceptions-testSoftAssertException.html");
		}

		[Test]
		public virtual void TestWatermarkExceptionNotAttached()
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

		[Test]
		public virtual void TestWatermarkExceptionWritingMessage()
		{
			// Driver will be closed, needs a new one for this test
			sm.ReplaceDriver(sm.CreateDriver());
			sm.Add(new WatermarkService());
			sm.Driver.Close();
			// Failure to write because the driver is closed causes error messages about the watermark, but not exception
			sm.WatermarkText("thisShouldNotFail");
			lfas.AssertLast("[WARN]", "Can't write onFailure watermark thisShouldNotFail. Message: invalid session id");
		}

		[Test]
		public virtual void TestWatermarkExceptionWritingOnFailureMessage()
		{
			// Driver will be closed, needs a new one for this test
			sm.ReplaceDriver(sm.CreateDriver());
			sm.Add(new WatermarkService());
			sm.WatermarkText("this works");
			// Failure to write because the driver is closed causes error messages about the watermark
			sm.Driver.Close();
			sm.OnFailure("thisClass", "thisTest");
			lfas.AssertLast("[ERROR]", "Can't write onFailure watermark thisTest. Message: invalid session id");
		}
	}
}
