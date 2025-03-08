using NUnit.Framework;
using OpenQA.Selenium;
using Giis.Selema.Manager;
using Giis.Selema.Portable.Selenium;
using Test4giis.Selema.Portable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Core
{
    /// <summary>
    /// Test on strategies to get the driver version.
    /// 
    /// This test class is deactivated by default and should be used for manual test only.
    /// Test result comparison should be made on the path and driver file downloaded
    /// </summary>
     [Ignore("Manual test to avoid side effects with tests executed after this")] public class TestDriverVersion
    {
        // Get a new SeleManager instance configured for a given version startegy
        private SeleManager NewSeleManager(string versionStrategy)
        {
            SeleManager sm = new SeleManager();
            if (new Config4test().UseHeadlessDriver())
                sm.SetArguments(TestDriver.chromeHeadlesArgument);
            sm.SetDriverVersion(versionStrategy);
            return sm;
        }

        [Test]
        public virtual void TestLocalDriverMatch()
        {
            SeleManager sm = NewSeleManager(DriverVersion.MATCH_BROWSER);
            IWebDriver driver = sm.CreateDriver("ThisClass", "ThisTest");
            DriverUtil.CloseDriver(driver);
        }

        [Test]
        public virtual void TestLocalDriverLatest()
        {
            SeleManager sm = NewSeleManager(DriverVersion.LATEST_AVAILABLE);
            IWebDriver driver = sm.CreateDriver("ThisClass", "ThisTest");
            DriverUtil.CloseDriver(driver);
        }

        // If some driver has already been downloaded and configured in the path, 
        // SeleniumManager will locate it.
        // If before execution, an old driver has been loaded by next test,
        // and configured in the path, the test will fail
        [Test]
        public virtual void TestLocalDriverSelenium()
        {
            SeleManager sm = NewSeleManager(DriverVersion.SELENIUM_MANAGER);
            IWebDriver driver = sm.CreateDriver("ThisClass", "ThisTest");
            DriverUtil.CloseDriver(driver);
        }

        // Forces an old driver incompatible with current browsers that will raise exception
        [Test]
        public virtual void TestLocalDriverGivenVersion()
        {
            SeleManager sm = NewSeleManager("99.0.4844.51");
            try
            {
                sm.CreateDriver("ThisClass", "ThisTest");
                NUnit.Framework.Legacy.ClassicAssert.Fail("Should fail");
            }
            catch (Exception e)
            {
                Asserts.AssertIsTrue(e.Message.Contains("This version of ChromeDriver only supports Chrome version 99"), "Not contained in: " + e.Message);
            }
        }
    }
}