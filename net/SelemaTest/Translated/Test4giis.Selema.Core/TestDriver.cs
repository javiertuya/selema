/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Giis.Portable.Util;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using Giis.Selema.Services.Impl;
using NUnit.Framework;
using OpenQA.Selenium;
using Sharpen;

namespace Test4giis.Selema.Core
{
	/// <summary>
	/// Main tests to check that drivers are created
	/// for diferent browsers, modes, arguments and parameters
	/// </summary>
	public class TestDriver
	{
		public static string[] chromeHeadlesArgument = new string[] { "--headless", "--remote-allow-origins=*" };

		//As of Chrome Driver V 111, we need to include remote-allow-origins argument, 
		//if not connection with driver fails
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
		public virtual void TestLocalWebDriverChrome()
		{
			if (!IsLocal())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			IWebDriver driver = factory.GetSeleniumDriver("chrome", string.Empty, string.Empty, null, new string[] { "--remote-allow-origins=*" }, null);
			AssertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--remote-allow-origins=*]}}");
			driver.Close();
		}

		[Test]
		public virtual void TestLocalWebDriverEdge()
		{
			if (!IsLocal())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			IWebDriver driver = factory.GetSeleniumDriver("edge", string.Empty, string.Empty, null, null, null);
			AssertOptions(factory, Parameters.IsJava() ? "{browserName:MicrosoftEdge,ms:edgeOptions:{args:[--remote-allow-origins=*]}}" : "{browserName:MicrosoftEdge,ms:edgeOptions:{}}");
			driver.Close();
		}

		[Test]
		public virtual void TestLocalWebDriverFirefox()
		{
			if (!IsLocal())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			IWebDriver driver = factory.GetSeleniumDriver("firefox", string.Empty, string.Empty, null, null, null);
			AssertOptions(factory, "{browserName:firefox,moz:firefoxOptions:{}}");
			driver.Close();
		}

		//next tests use chrome
		[Test]
		public virtual void TestHeadlessWebDriverDefault()
		{
			if (!UseHeadless())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			IWebDriver driver = factory.GetSeleniumDriver("chrome", string.Empty, string.Empty, null, chromeHeadlesArgument, null);
			AssertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}");
			driver.Close();
			//browser name is case insensitive, browser already downloaded, null remote url
			driver = factory.GetSeleniumDriver("CHRome", null, null, null, chromeHeadlesArgument, null);
			AssertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}");
			driver.Close();
		}

		[Test]
		public virtual void TestHeadlessWebDriverWithOptions()
		{
			if (!UseHeadless())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			IDictionary<string, object> caps = new SortedDictionary<string, object>();
			//As of Selenium 4.9.0 non standard capabilities must have a prefix
			caps["testprefix:key1"] = "value1";
			caps["testprefix:key2"] = "value2";
			//caps.put("unhandledPromptBehavior", "ignore");
			IWebDriver driver = factory.GetSeleniumDriver("chrome", string.Empty, string.Empty, caps, chromeHeadlesArgument, null);
			AssertOptions(factory, Parameters.IsJava() ? "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]},testprefix:key1:value1,testprefix:key2:value2}" : "{browserName:chrome,testprefix:key1:value1,testprefix:key2:value2,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}"
				);
			//different order on net
			driver.Close();
		}

		//testHeadlessWebDriverWithOptionsAndOptionsInstance tested in separate class (transformed manually to C#)
		[Test]
		public virtual void TestHeadlessWebDriverNotFound()
		{
			if (!UseHeadless())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			try
			{
				factory.GetSeleniumDriver("carome", string.Empty, string.Empty, null, chromeHeadlesArgument, null);
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch (SelemaException e)
			{
				Asserts.AssertIsTrue(e.Message.StartsWith("Can't instantiate WebDriver Options for browser: carome"), "Not contained in: " + e.Message);
			}
		}

		[Test]
		public virtual void TestHeadlessWebDriverUnloadable()
		{
			if (!UseHeadless())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			try
			{
				factory.EnsureLocalDriverDownloaded("corome", string.Empty);
				NUnit.Framework.Assert.Fail("Should fail");
			}
			catch (SelemaException e)
			{
				Asserts.AssertIsTrue(e.Message.StartsWith("Can't download driver executable for browser: corome"), "Not contained in: " + e.Message);
			}
		}

		//Custom assertion to allow same comparisons in java and net
		private void AssertOptions(SeleniumDriverFactory factory, string expected)
		{
			expected = expected.Replace(" ", string.Empty);
			string actual = factory.GetLastOptionString().Replace(" ", string.Empty);
			if (Parameters.IsJava())
			{
				actual = actual.Replace("Capabilities{", "{").Replace(",extensions:[]", string.Empty).Replace("acceptInsecureCerts:true,", string.Empty).Replace("moz:debuggerAddress:true,", string.Empty);
			}
			if (Parameters.IsNetCore())
			{
				actual = actual.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\"", string.Empty);
			}
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}

		[Test]
		public virtual void TestRemoteWebDriverDefault()
		{
			if (!UseRemote())
			{
				return;
			}
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			IWebDriver driver = factory.GetSeleniumDriver("chrome", new Config4test().GetRemoteDriverUrl(), null, null, null, null);
			//NOTE: Selenium 4.8.2/3 (java) adds --remote-allow-origins=* to prevent the Chrome Driver 111 breaking change
			//As of Selenium 4.14.1 remote-allow-origins is removed
			AssertOptions(factory, Parameters.IsJava() ? "{browserName:chrome,goog:chromeOptions:{args:[]}}" : "{browserName:chrome,goog:chromeOptions:{}}");
			driver.Close();
		}

		[Test]
		public virtual void TestRemoteWebDriverWithArguments()
		{
			if (!UseRemote())
			{
				return;
			}
			//setting options has been tested with local
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			IWebDriver driver = factory.GetSeleniumDriver("chrome", new Config4test().GetRemoteDriverUrl(), null, null, new string[] { "--start-maximized" }, null);
			//NOTE: Selenium 4.8.2/3 (java) adds --remote-allow-origins=* to prevent the Chrome Driver 111 breaking change
			//As of Selenium 4.14.1 remote-allow-origins is removed
			AssertOptions(factory, Parameters.IsJava() ? "{browserName:chrome,goog:chromeOptions:{args:[--start-maximized]}}" : "{browserName:chrome,goog:chromeOptions:{args:[--start-maximized]}}");
			driver.Close();
		}

		[Test]
		public virtual void TestRemoteWebDriverWrongUrl()
		{
			if (!UseRemote())
			{
				return;
			}
			string wrongUrl = new Config4test().GetRemoteDriverUrl() + "/notexist";
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			try
			{
				factory.GetSeleniumDriver("chrome", wrongUrl, null, null, null, null);
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
			if (!UseRemote())
			{
				return;
			}
			IDictionary<string, object> capsToAdd = new SortedDictionary<string, object>();
			capsToAdd["testprefix:key1"] = "value1";
			capsToAdd["testprefix:key2"] = "value2";
			SeleManager sm = new SeleManager(Config4test.GetConfig()).SetDriverUrl(new Config4test().GetRemoteDriverUrl()).SetOptions(capsToAdd);
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
			if (!UseRemote())
			{
				return;
			}
			SeleManager sm = new SeleManager(Config4test.GetConfig()).SetDriverUrl(new Config4test().GetRemoteDriverUrl()).Add(new SelenoidService());
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
