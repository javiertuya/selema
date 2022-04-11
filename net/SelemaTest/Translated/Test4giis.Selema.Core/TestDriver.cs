/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using Giis.Selema.Portable;
using Giis.Selema.Services.Impl;
using NUnit.Framework;
using OpenQA.Selenium;
using Sharpen;

namespace Test4giis.Selema.Core
{
	/// <summary>Some detailed tests for the driver instantiation features</summary>
	public class TestDriver
	{
		private string[] headlesArgument = new string[] { "--headless" };

		//Not all tests can be executed in all test modes,
		//all in local, remote driver on selenoid, local driver on headless
		private bool OnRemote()
		{
			return new CiServiceFactory().GetCurrent().IsLocal() || new Config4test().UseRemoteWebDriver();
		}

		private bool OnLocal()
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
		public virtual void TestLocalWebDriverDefault()
		{
			if (!OnLocal())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			IWebDriver driver = factory.GetSeleniumDriver("chrome", string.Empty, null, headlesArgument);
			AssertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--headless]}}");
			driver.Close();
			//browser name is case insensitive, browser already downloaded, null remote url
			driver = factory.GetSeleniumDriver("CHRome", null, null, headlesArgument);
			AssertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--headless]}}");
			driver.Close();
		}

		[Test]
		public virtual void TestLocalWebDriverWithOptions()
		{
			if (!OnLocal())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			IDictionary<string, object> caps = new SortedDictionary<string, object>();
			caps["key1"] = "value1";
			caps["key2"] = "value2";
			IWebDriver driver = factory.GetSeleniumDriver("chrome", string.Empty, caps, headlesArgument);
			AssertOptions(factory, new SelemaConfig().IsJava() ? "{browserName:chrome,goog:chromeOptions:{args:[--headless]},key1:value1,key2:value2}" : "{browserName:chrome,key1:value1,key2:value2,goog:chromeOptions:{args:[--headless]}}");
			//different order on net
			driver.Close();
		}

		[Test]
		public virtual void TestLocalWebDriverNotFound()
		{
			if (!OnLocal())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			try
			{
				factory.GetSeleniumDriver("carome", string.Empty, null, headlesArgument);
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch (SelemaException e)
			{
				NUnit.Framework.Assert.IsTrue(e.Message.StartsWith("Can't instantiate WebDriver Options for browser: carome"));
			}
		}

		[Test]
		public virtual void TestLocalWebDriverUnloadable()
		{
			if (!OnLocal())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			try
			{
				factory.EnsureLocalDriverDownloaded("corome");
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch (SelemaException e)
			{
				NUnit.Framework.Assert.IsTrue(e.Message.StartsWith("Can't download driver executable for browser: corome"));
			}
		}

		//Custom assertion to allow same comparisons in java and net
		private void AssertOptions(SeleniumDriverFactory factory, string expected)
		{
			SelemaConfig conf = new SelemaConfig();
			expected = expected.Replace(" ", string.Empty);
			string actual = factory.GetLastOptionString().Replace(" ", string.Empty);
			if (conf.IsJava())
			{
				actual = actual.Replace("Capabilities{", "{").Replace(",extensions:[]", string.Empty).Replace("acceptInsecureCerts:true,", string.Empty).Replace("moz:debuggerAddress:true,", string.Empty);
			}
			if (conf.IsNet())
			{
				actual = actual.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\"", string.Empty);
			}
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}

		[Test]
		public virtual void TestRemoteWebDriverDefault()
		{
			if (!OnRemote())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			IWebDriver driver = factory.GetSeleniumDriver("chrome", new Config4test().GetRemoteDriverUrl(), null, null);
			AssertOptions(factory, new SelemaConfig().IsJava() ? "{browserName:chrome,goog:chromeOptions:{args:[]}}" : "{browserName:chrome,goog:chromeOptions:{}}");
			driver.Close();
		}

		[Test]
		public virtual void TestRemoteWebDriverWithArguments()
		{
			if (!OnRemote())
			{
				return;
			}
			//setting options has been tested with local
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			IWebDriver driver = factory.GetSeleniumDriver("chrome", new Config4test().GetRemoteDriverUrl(), null, new string[] { "--start-maximized" });
			AssertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--start-maximized]}}");
			driver.Close();
		}

		[Test]
		public virtual void TestRemoteWebDriverWrongUrl()
		{
			if (!OnRemote())
			{
				return;
			}
			string wrongUrl = new Config4test().GetRemoteDriverUrl() + "/notexist";
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			try
			{
				factory.GetSeleniumDriver("chrome", wrongUrl, null, null);
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch (SelemaException e)
			{
				Asserts.AssertIsTrue(e.Message.StartsWith("Can't instantiate RemoteWebDriver for browser: chrome at url: " + wrongUrl), "Not contained in: " + e.Message);
			}
		}

		//lifecycle tests with remote driver use a browser service, but it should work if not browser service is attached
		//As this is not inside a lifecycle controller, simulates the steps.
		[Test]
		public virtual void TestRemoteWebDriverFromManagerNoBrowserService()
		{
			if (!OnRemote())
			{
				return;
			}
			IDictionary<string, object> capsToAdd = new SortedDictionary<string, object>();
			capsToAdd["key1"] = "value1";
			capsToAdd["key2"] = "value2";
			SeleniumManager sm = new SeleniumManager(Config4test.GetConfig()).SetDriverUrl(new Config4test().GetRemoteDriverUrl()).SetOptions(capsToAdd);
			//can't get options from driver instance, check at the debug log
			sm.OnSetUp("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
			sm.OnFailure("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
			AssertLogRemoteWebDriver();
			sm.OnTearDown("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
		}

		//with browser service but no video too
		[Test]
		public virtual void TestRemoteWebDriverFromManagerNoVideoService()
		{
			if (!OnRemote())
			{
				return;
			}
			SeleniumManager sm = new SeleniumManager(Config4test.GetConfig()).SetDriverUrl(new Config4test().GetRemoteDriverUrl()).Add(new SelenoidService());
			sm.OnSetUp("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
			sm.OnFailure("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
			AssertLogRemoteWebDriver();
			sm.OnTearDown("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
		}

		private void AssertLogRemoteWebDriver()
		{
			LogReader logReader = new LifecycleAsserts().GetLogReader();
			logReader.AssertBegin();
			logReader.AssertContains("Creating SeleniumManager instance");
			logReader.AssertContains("*** SetUp - TestDriver.testRemoteWebDriverFromManager");
			logReader.AssertContains("Remote session chrome starting");
			logReader.AssertContains("Remote session chrome started. Remote web driver at " + new Config4test().GetRemoteDriverUrl());
			logReader.AssertContains("FAIL TestDriver.testRemoteWebDriverFromManager");
			logReader.AssertContains("Taking screenshot", "TestDriver-testRemoteWebDriverFromManager.png");
			logReader.AssertEnd();
		}
	}
}
