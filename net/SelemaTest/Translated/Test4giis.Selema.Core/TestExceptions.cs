using NUnit.Framework;
using OpenQA.Selenium;
using NLog;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using Giis.Selema.Portable.Selenium;
using Giis.Selema.Services;
using Giis.Selema.Services.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Core
{
    /// <summary>
    /// Checks exceptional situations: Some of them do not raise exceptions, but write in the selema log,
    /// others should raise exception to the user.
    /// Note that exceptions are tested using try catch to allow automatic conversion to nunit
    /// </summary>
    [LifecycleNunit3] public class TestExceptions : IAfterEachCallback
    {
        //interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
        static readonly Logger log = LogManager.GetCurrentClassLogger(); //TestExceptions));
        protected static LifecycleAsserts lfas = new LifecycleAsserts();
        protected virtual string CurrentName()
        {
            return "TestExceptions";
        }

        protected static int thisTestCount = 0;
        //this sm includes a configuration of the driver (check that driver runs maximized)
        protected internal SeleManager sm = new SeleManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test(false)).SetManageAtClass().SetDriverDelegate(new DriverConfigMaximize());
        //public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
        //public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm);
        protected static IWebDriver saveDriver;
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
            sm.Add((WatermarkService)null); // some tests add this service, this removes it
            lfas.AssertAfterSetup(sm, false, thisTestCount == 0); //ensures correct setup
            LaunchPage();
        }

        [NUnit.Framework.TearDown]
        public virtual void TearDown()
        {
            thisTestCount++; //only 0 in first test
            sm.ReplaceDriver(saveDriver);
        }

        protected virtual void LaunchPage()
        {
            DriverUtil.GetUrl(sm.Driver, new Config4test().GetWebUrl()); //siempre usa la misma pagina
        }

        [Test]
        public virtual void TestManagerWithoutConfig()
        {
            try
            {
                new SeleManager(null);
                NUnit.Framework.Legacy.ClassicAssert.Fail("Should fail");
            }
            catch (SelemaException e)
            {
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("SeleManager instance requires an instance of SelemaConfig", e.Message);
            }
        }

        //uses a different report subdir to do not include wrong named files/folders that cause error when published as artifacts
        [Test]
        public virtual void TestManagerWrongName()
        {
            try
            {
                new SeleManager(new SelemaConfig().SetReportSubdir("dat/tmp").SetName("ab\0"));
                NUnit.Framework.Legacy.ClassicAssert.Fail("Should fail");
            }
            catch (Exception e)
            {
            } //NOSONAR
            //NOSONAR
        }

        [Test]
        public virtual void TestManagerWrongReportSubdir()
        {
            try
            {
                new SeleManager(new SelemaConfig().SetReportSubdir("dat/tmp/ab\0"));
                NUnit.Framework.Legacy.ClassicAssert.Fail("Should fail");
            }
            catch (Exception e)
            {
            } //NOSONAR
            //NOSONAR
        }

        [Test]
        public virtual void TestManagerWrongProjectRoot()
        {
            try
            {
                new SeleManager(new SelemaConfig().SetProjectRoot("dat/tmp/ab\0"));
                NUnit.Framework.Legacy.ClassicAssert.Fail("Should fail");
            }
            catch (Exception e)
            {
            } //NOSONAR
            //NOSONAR
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
            IMediaContext context = new MediaContext(sm.GetConfig().GetProjectRoot() + "/dat/tmp/ab\0", sm.GetConfig().GetQualifier(), 99, 99);
            sm.GetScreenshotService().TakeScreenshot(sm.Driver, context, "TestExceptions.testScreenshotInternalException");
            lfas.AssertLast("[ERROR]", "Can't take screenshot or write the content to file", "TestExceptions-testScreenshotInternalException.png");
        }

        [Test]
        public virtual void TestVisualAssertException()
        {
            sm.VisualAssertEquals("ab cd", "ab cd"); //first assert pass
            try
            {
                sm.VisualAssertEquals("ab cd", "ab xy cd");
                NUnit.Framework.Legacy.ClassicAssert.Fail("Should fail");
            }
            catch (Exception e)
            {
            } //NOSONAR

            lfas.AssertLast("[WARN]", "Visual Assert differences", "TestExceptions-testVisualAssertException.html");

            //with message
            try
            {
                sm.VisualAssertEquals("ef gh", "ef zt gh", "va message");
                NUnit.Framework.Legacy.ClassicAssert.Fail("Should fail");
            }
            catch (Exception e)
            {
            } //NOSONAR

            lfas.AssertLast("[WARN]", "Visual Assert differences", "TestExceptions-testVisualAssertException.html", "va message");
        }

        [Test]
        public virtual void TestSoftAssertException()
        {
            sm.SoftAssertClear();
            sm.SoftAssertEquals("ab cd", "ab cd"); //first assert pass
            sm.SoftAssertEquals("ab cd", "ab xy cd");
            sm.SoftAssertEquals("ef gh", "ef zt gh", "sva message");
            try
            {
                sm.SoftAssertAll();
                NUnit.Framework.Legacy.ClassicAssert.Fail("Should fail");
            }
            catch (Exception e)
            {
            } //NOSONAR

            lfas.AssertLast(0, "[WARN]", "Soft Visual Assert differences (Failure 2)", "TestExceptions-testSoftAssertException.html", "sva message");
            lfas.AssertLast(1, "[WARN]", "Soft Visual Assert differences (Failure 1)", "TestExceptions-testSoftAssertException.html");
        }

        [Test]
        public virtual void TestWatermarkExceptionNotAttached()
        {
            try
            {
                sm.Watermark();
                NUnit.Framework.Legacy.ClassicAssert.Fail("Should fail");
            }
            catch (Exception e)
            {
            } //NOSONAR

            lfas.AssertLast("[ERROR]", "Watermark service is not attached to this Selenium Manager");
        }

        [Test]
        public virtual void TestWatermarkExceptionWritingMessage()
        {

            // Driver will be closed, needs a new one for this test
            sm.ReplaceDriver(sm.CreateDriver());
            sm.Add(new WatermarkService());
            sm.Driver.Dispose();

            // Failure to write because the driver is closed causes error messages about the watermark, but not exception
            sm.WatermarkText("thisShouldNotFail");
            lfas.AssertLast("[WARN]", "Can't write onFailure watermark thisShouldNotFail");
        }

        [Test]
        public virtual void TestWatermarkExceptionWritingOnFailureMessage()
        {

            // Driver will be closed, needs a new one for this test
            sm.ReplaceDriver(sm.CreateDriver());
            sm.Add(new WatermarkService());
            sm.WatermarkText("this works");

            // Failure to write because the driver is closed causes error messages about the watermark
            sm.Driver.Dispose();
            sm.OnFailure("thisClass", "thisTest");
            lfas.AssertLast("[ERROR]", "Can't write onFailure watermark thisTest");
        }
    }
}