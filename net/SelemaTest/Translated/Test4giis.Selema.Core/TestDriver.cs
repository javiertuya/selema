using Java.Util;
using NUnit.Framework;
using OpenQA.Selenium;
using Giis.Portable.Util;
using Giis.Selema.Manager;
using Giis.Selema.Portable.Selenium;
using Giis.Selema.Services.Impl;
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
    /// Main tests to check that drivers are created
    /// for diferent browsers, modes, arguments and parameters
    /// </summary>
    public class TestDriver
    {
        //As of Chrome Driver V 111, we need to include remote-allow-origins argument, 
        //if not connection with driver fails
        public static string[] chromeHeadlesArgument = new string[]
        {
            "--headless",
            "--remote-allow-origins=*"
        };
        //Not all tests can be executed in all test modes,
        //all in local plus remote driver in CI, headless in local
        public static bool IsLocal()
        {
            return new CiServiceFactory().GetCurrent().IsLocal();
        }

        public static bool UseRemote()
        {
            return new CiServiceFactory().GetCurrent().IsLocal() || new Config4test().UseRemoteWebDriver();
        }

        public static bool UseHeadless()
        {
            return new CiServiceFactory().GetCurrent().IsLocal() || new Config4test().UseHeadlessDriver();
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp()
        {
        }

        [NUnit.Framework.TearDown]
        public virtual void TearDown()
        {
        }

        [Test]
        public virtual void TestLocalIWebDriverChrome()
        {
            if (!IsLocal())
                return;
            SeleniumDriverFactory factory = new SeleniumDriverFactory();
            IWebDriver driver = factory.GetSeleniumDriver("chrome", "", "", null, new string[] { "--remote-allow-origins=*" }, null);
            AssertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--remote-allow-origins=*]}}");
            DriverUtil.CloseDriver(driver);
        }

        [Test]
        public virtual void TestLocalIWebDriverEdge()
        {
            if (!IsLocal())
                return;
            SeleniumDriverFactory factory = new SeleniumDriverFactory();
            IWebDriver driver = factory.GetSeleniumDriver("edge", "", "", null, null, null);
            AssertOptions(factory, Parameters.IsJava() ? "{browserName:MicrosoftEdge,ms:edgeOptions:{args:[]}}" : "{browserName:MicrosoftEdge,ms:edgeOptions:{}}");
            DriverUtil.CloseDriver(driver);
        }

        [Test]
        public virtual void TestLocalIWebDriverFirefox()
        {
            if (!IsLocal())
                return;
            SeleniumDriverFactory factory = new SeleniumDriverFactory();
            IWebDriver driver = factory.GetSeleniumDriver("firefox", "", "", null, null, null);

            // #801 no toString method implemented for firefox
            AssertOptions(factory, Parameters.IsJava() ? "{browserName:firefox,moz:firefoxOptions:{prefs:{remote.active-protocols:3}}}" : "OpenQA.Selenium.Firefox.FirefoxOptions");
            DriverUtil.CloseDriver(driver);
        }

        //next tests use chrome
        [Test]
        public virtual void TestHeadlessIWebDriverDefault()
        {
            if (!UseHeadless())
                return;
            SeleniumDriverFactory factory = new SeleniumDriverFactory();
            IWebDriver driver = factory.GetSeleniumDriver("chrome", "", "", null, chromeHeadlesArgument, null);
            AssertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}");
            DriverUtil.CloseDriver(driver);

            //browser name is case insensitive, browser already downloaded, null remote url
            driver = factory.GetSeleniumDriver("CHRome", null, null, null, chromeHeadlesArgument, null);
            AssertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}");
            DriverUtil.CloseDriver(driver);
        }

        [Test]
        public virtual void TestHeadlessIWebDriverWithOptions()
        {
            if (!UseHeadless())
                return;
            SeleniumDriverFactory factory = new SeleniumDriverFactory();
            Map<string, object> caps = new TreeMap<string, object>();

            //As of Selenium 4.9.0 non standard capabilities must have a prefix
            caps.Put("testprefix:key1", "value1");
            caps.Put("testprefix:key2", "value2");
            IWebDriver driver = factory.GetSeleniumDriver("chrome", "", "", caps, chromeHeadlesArgument, null);

            // #801 the toString method is not able to get other custom capabilities than the standard and chromeOptions
            AssertOptions(factory, Parameters.IsJava() ? "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]},testprefix:key1:value1,testprefix:key2:value2}" : "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}");
            DriverUtil.CloseDriver(driver);
        }

        //testHeadlessIWebDriverWithOptionsAndOptionsInstance tested in separate class (transformed manually to C#)
        [Test]
        public virtual void TestHeadlessIWebDriverNotFound()
        {
            if (!UseHeadless())
                return;
            SeleniumDriverFactory factory = new SeleniumDriverFactory();
            Exception e = NUnit.Framework.Assert.Throws(typeof(SelemaException), () =>
            {
                factory.GetSeleniumDriver("carome", "", "", null, chromeHeadlesArgument, null);
            });
            Asserts.AssertIsTrue(e.Message.StartsWith("Can't instantiate IWebDriver Options for browser: carome".Replace("IWeb", "Web")), "Not contained in: " + e.Message);
        }

        [Test]
        public virtual void TestHeadlessIWebDriverUnloadable()
        {
            if (!UseHeadless())
                return;
            SeleniumDriverFactory factory = new SeleniumDriverFactory();
            Exception e = NUnit.Framework.Assert.Throws(typeof(SelemaException), () =>
            {
                factory.EnsureLocalDriverDownloaded("corome", "");
            });
            Asserts.AssertIsTrue(e.Message.StartsWith("Can't download driver executable for browser: corome"), "Not contained in: " + e.Message);
        }

        //Custom assertion to allow same comparisons in java and net
        private void AssertOptions(SeleniumDriverFactory factory, string expected)
        {
            expected = expected.Replace(" ", "");
            string actual = factory.GetLastOptionString().Replace(" ", "");
            if (Parameters.IsJava())
                actual = actual.Replace("DriverOptions{", "{").Replace(",extensions:[]", "").Replace("acceptInsecureCerts:true,", "").Replace("moz:debuggerAddress:true,", "");
            if (Parameters.IsNetCore())
                actual = actual.Replace("\n", "").Replace("\r", "").Replace("\"", "");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(expected, actual);
        }

        [Test]
        public virtual void TestRemoteWebDriverDefault()
        {
            if (!UseRemote())
                return;
            SeleniumDriverFactory factory = new SeleniumDriverFactory();
            IWebDriver driver = factory.GetSeleniumDriver("chrome", new Config4test().GetRemoteDriverUrl(), null, null, null, null);

            //NOTE: Selenium 4.8.2/3 (java) adds --remote-allow-origins=* to prevent the Chrome Driver 111 breaking change
            //As of Selenium 4.14.1 remote-allow-origins is removed
            AssertOptions(factory, Parameters.IsJava() ? "{browserName:chrome,goog:chromeOptions:{args:[]}}" : "{browserName:chrome,goog:chromeOptions:{}}");
            DriverUtil.CloseDriver(driver);
        }

        [Test]
        public virtual void TestRemoteWebDriverWithArguments()
        {
            if (!UseRemote())
                return;

            //setting options has been tested with local
            SeleniumDriverFactory factory = new SeleniumDriverFactory();
            IWebDriver driver = factory.GetSeleniumDriver("chrome", new Config4test().GetRemoteDriverUrl(), null, null, new string[] { "--start-maximized" }, null);

            //NOTE: Selenium 4.8.2/3 (java) adds --remote-allow-origins=* to prevent the Chrome Driver 111 breaking change
            //As of Selenium 4.14.1 remote-allow-origins is removed
            AssertOptions(factory, Parameters.IsJava() ? "{browserName:chrome,goog:chromeOptions:{args:[--start-maximized]}}" : "{browserName:chrome,goog:chromeOptions:{args:[--start-maximized]}}");
            DriverUtil.CloseDriver(driver);
        }

        [Test]
        public virtual void TestRemoteWebDriverWrongUrl()
        {
            if (!UseRemote())
                return;
            string wrongUrl = new Config4test().GetRemoteDriverUrl() + "/notexist";
            SeleniumDriverFactory factory = new SeleniumDriverFactory();
            Exception e = NUnit.Framework.Assert.Throws(typeof(SelemaException), () =>
            {
                factory.GetSeleniumDriver("chrome", wrongUrl, null, null, null, null);
            });
            Asserts.AssertIsTrue(e.Message.StartsWith("Can't instantiate RemoteWebDriver for browser: chrome at url: " + wrongUrl), "Not contained in: " + e.Message);
        }

        //lifecycle tests with remote driver use a browser service, but it should work if not browser service is attached
        //As this is not inside a lifecycle controller, simulates the steps.
        [Test]
        public virtual void TestRemoteWebDriverFromManagerNoBrowserService()
        {
            if (!UseRemote())
                return;
            Map<string, object> capsToAdd = new TreeMap<string, object>();
            capsToAdd.Put("testprefix:key1", "value1");
            capsToAdd.Put("testprefix:key2", "value2");
            SeleManager sm = new SeleManager(Config4test.GetConfig()).SetDriverUrl(new Config4test().GetRemoteDriverUrl()).SetOptions(capsToAdd); //can't get options from driver instance, check at the debug log
            sm.OnSetUp("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
            sm.OnFailure("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
            AssertLogRemoteWebDriver();
            sm.OnTearDown("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
        }

        //with browser service but no video too
        [Test]
        public virtual void TestRemoteWebDriverFromManagerNoVideoWithCapability()
        {
            if (!UseRemote())
                return;
            SeleManager sm = new SeleManager(Config4test.GetConfig()).SetDriverUrl(new Config4test().GetRemoteDriverUrl()).Add(new SelenoidService().SetCapability("enableLog", true));
            sm.OnSetUp("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
            sm.OnFailure("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
            AssertLogRemoteWebDriver();
            sm.OnTearDown("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
        }

        private void AssertLogRemoteWebDriver()
        {
            LogReader logReader = new LifecycleAsserts().GetLogReader();
            logReader.AssertBegin();
            logReader.AssertContains("Creating SeleManager instance");
            logReader.AssertContains("*** SetUp - TestDriver.testRemoteWebDriverFromManager");
            logReader.AssertContains("Remote session chrome starting");
            logReader.AssertContains("Remote session chrome started. Remote web driver at " + new Config4test().GetRemoteDriverUrl());
            logReader.AssertContains("FAIL TestDriver.testRemoteWebDriverFromManager");
            logReader.AssertContains("Taking screenshot", "TestDriver-testRemoteWebDriverFromManager.png");
            logReader.AssertEnd();
        }
    }
}