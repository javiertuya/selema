/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Giis.Selema.Manager;
using Giis.Selema.Portable;
using Giis.Selema.Services.Impl;
using Java.Util;
using Sharpen;

namespace Test4giis.Selema.Core
{
	public class Config4test : IManagerConfigDelegate
	{
		private string PropertiesFile = FileUtil.GetPath(new SelemaConfig().GetProjectRoot(), "selema.properties");

		private Properties prop;

		public Config4test()
		{
			prop = new PropertiesFactory().GetPropertiesFromFilename(PropertiesFile);
			if (prop == null)
			{
				throw new Exception("Can't load test configuration: selema.properties");
			}
		}

		//default selema config for tests
		public static SelemaConfig GetConfig()
		{
			//by default test reports located in subfolder
			return new SelemaConfig().SetReportSubdir(FileUtil.GetPath(Parameters.DefaultReportSubdir + "/selema"));
		}

		//implements the interface IManagerConfig to establish the default configuration for test
		public virtual void Configure(SeleniumManager sm)
		{
			sm.SetBrowser(GetCurrentBrowser()).SetDriverUrl(GetCurrentDriverUrl()).Add(new WatermarkService());
			if (UseHeadlessDriver())
			{
				sm.SetArguments(new string[] { "--headless" });
			}
			if (UseRemoteWebDriver())
			{
				sm.Add(new SelenoidService().SetVideo().SetVnc());
			}
		}

		public virtual bool UseRemoteWebDriver()
		{
			return "selenoid".Equals(prop.GetProperty("selema.test.mode"));
		}

		public virtual bool UseHeadlessDriver()
		{
			return "headless".Equals(prop.GetProperty("selema.test.mode"));
		}

		private string GetRemoteDriverUrl()
		{
			return prop.GetProperty("selema.test.remote.webdriver");
		}

		public virtual string GetCurrentDriverUrl()
		{
			return UseRemoteWebDriver() ? GetRemoteDriverUrl() : string.Empty;
		}

		public virtual string GetCurrentBrowser()
		{
			return prop.GetProperty("selema.test.browser");
		}

		public virtual string GetWebRoot()
		{
			return prop.GetProperty("selema.test.web.root");
		}

		public virtual string GetWebUrl()
		{
			return GetWebRoot() + "/main/actions.html";
		}

		public virtual string GetCoverageUrl()
		{
			return GetWebRoot() + "/instrumented/actions.html";
		}

		public virtual string GetCoverageRoot()
		{
			return GetWebRoot() + "/instrumented";
		}

		public virtual bool GetManualCheckEnabled()
		{
			return "true".Equals(prop.GetProperty("selema.test.manual.check"));
		}
	}
}
