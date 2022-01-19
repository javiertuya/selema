/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using Giis.Selema.Portable;
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
			driver = factory.GetSeleniumDriver("firefox", string.Empty, null, headlesArgument);
			AssertOptions(factory, "{browserName:firefox,moz:firefoxOptions:{args:[--headless]}}");
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
		public virtual void TestRemoteWebDriverFromManager()
		{
			if (!OnRemote())
			{
				return;
			}
			LogReader logReader = new LifecycleAsserts().GetLogReader();
			SeleniumManager sm = new SeleniumManager(Config4test.GetConfig()).SetDriverUrl(new Config4test().GetRemoteDriverUrl());
			sm.OnSetUp("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
			sm.OnFailure("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
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
