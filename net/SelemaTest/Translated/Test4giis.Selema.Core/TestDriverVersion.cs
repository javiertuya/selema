/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Giis.Selema.Manager;
using NUnit.Framework;
using OpenQA.Selenium;
using Sharpen;
using Test4giis.Selema.Portable;

namespace Test4giis.Selema.Core
{
	/// <summary>Test on strategies to get the driver version.</summary>
	/// <remarks>
	/// Test on strategies to get the driver version.
	/// This test class is deactivated by default and should be used for manual test only.
	/// Test result comparison should be made on the path and driver file downloaded
	/// </remarks>
	 [Ignore("Manual Test")] public class TestDriverVersion
	{
		// Get a new SeleManager instance configured for a given version startegy
		private SeleManager NewSeleManager(string versionStrategy)
		{
			SeleManager sm = new SeleManager();
			if (new Config4test().UseHeadlessDriver())
			{
				sm.SetArguments(TestDriver.chromeHeadlesArgument);
			}
			sm.SetDriverVersion(versionStrategy);
			return sm;
		}

		[Test]
		public virtual void TestLocalDriverMatch()
		{
			SeleManager sm = NewSeleManager(DriverVersion.MatchBrowser);
			IWebDriver driver = sm.CreateDriver("ThisClass", "ThisTest");
			driver.Close();
		}

		[Test]
		public virtual void TestLocalDriverLatest()
		{
			SeleManager sm = NewSeleManager(DriverVersion.LatestAvailable);
			IWebDriver driver = sm.CreateDriver("ThisClass", "ThisTest");
			driver.Close();
		}

		// If some driver has already been downloaded and configured in the path, 
		// SeleniumManager will locate it.
		// If before execution, an old driver has been loaded by next test,
		// and configured in the path, the test will fail
		[Test]
		public virtual void TestLocalDriverSelenium()
		{
			SeleManager sm = NewSeleManager(DriverVersion.SeleniumManager);
			IWebDriver driver = sm.CreateDriver("ThisClass", "ThisTest");
			driver.Close();
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
