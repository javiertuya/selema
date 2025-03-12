using System.Collections.Generic;
using Giis.Selema.Manager;
using Java.Util;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Test4giis.Selema.Core
{
    /// <summary>
    /// This test is java only, contains many dependencies that are difficult to be translated
    /// </summary>
    public class TestDriverPlatformDependent
	{

		[Test]
		public virtual void TestHeadlessWebDriverWithOptionsAndOptionsInstance()
		{
			if (!TestDriver.UseHeadless())
				return;
			SeleniumDriverFactory factory = new SeleniumDriverFactory();
			Map<string, object> caps = new TreeMap<string, object>();
			caps["testprefix:key1"] = "value1";
			ChromeOptions optInstance=new ChromeOptions();
			optInstance.UnhandledPromptBehavior = OpenQA.Selenium.UnhandledPromptBehavior.Ignore;
			IWebDriver driver = factory.GetSeleniumDriver("chrome", string.Empty, string.Empty, caps, TestDriver.chromeHeadlesArgument, optInstance);
            // #801 the toString method is not able to get other custom capabilities than the standard and chromeOptions
            Assert.AreEqual("{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}",
				factory.GetLastOptionString().Replace("\"","").Replace("\r","").Replace("\n","").Replace(" ",""));
			driver.Close();
		}
	}
}
