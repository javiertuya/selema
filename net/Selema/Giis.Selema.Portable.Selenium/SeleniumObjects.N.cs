using WebDriverManager.Helpers;
using WebDriverManager;
using WebDriverManager.DriverConfigs;

using Giis.Selema.Manager;
using NLog;
using System;
using System.IO;
using System.Reflection;
using OpenQA.Selenium.Chromium;

namespace Giis.Selema.Portable.Selenium
{
	/// <summary>
	/// Generic browser independent methods to create a configure a Selenium WebDriver:
	/// Uses reflection to obtain the different objects involved in this task.
	/// </summary>
	public class SeleniumObjects
	{
		internal readonly Logger log = LogManager.GetCurrentClassLogger();

        public object GetOptionsObj(string browser, string[] arguments)
        {
            string clasName = GetSeleniumClassName(browser, "Options");
            return Activator.CreateInstance(Type.GetType(clasName));
        }

        // #801 The toString gets all options in java, but needs a custom implementation in net
		// because it was removed in version 2.7.0
		// Custom implementation only for chrome and edge to generate the same string than java generates,
		// including part of the options (other additional capabilities can't be obtained because they are private)
        public string GetOptionsObjAsString(object obj)
		{
			if (obj is ChromiumOptions)
				return GetChromiumOptionsObjAsString((ChromiumOptions)obj);
			return obj.ToString();
        }
        public string GetChromiumOptionsObjAsString(ChromiumOptions optObj)
        {
            string arguments = optObj.Arguments.Count == 0 ? "" : "args:[" + string.Join(",", optObj.Arguments) + "]";
            string optString = "{"
                + "browserName:" + optObj.BrowserName
                + "," + optObj.CapabilityName + ":{" + arguments + "}"
                + "}";
            return optString;
        }

        public void SetCapability(object opt, string key, object value)
		{
			MethodInfo setCapability = opt.GetType().GetMethod("AddAdditionalOption", new Type[] { typeof(string), typeof(object) });
			setCapability.Invoke(opt, new object[] { key, value });
		}
		public void SetCapabilityV3(object opt, string key, object value)
		{
			//Rename this method to SetCapability if using selenium 3
			//As of selenium web driver 4.3.0 this method does not work as the deprecated 
			//AddAdditionalCapability method has been removed
			MethodInfo setCapability = opt.GetType().GetMethod("AddAdditionalCapability", new Type[] { typeof(string), typeof(object), typeof(bool) });
			setCapability.Invoke(opt, new object[] { key, value, true });
		}

		public void AddArguments(object opt, string[] args)
		{
			MethodInfo addArgument = opt.GetType().GetMethod("AddArgument");
			foreach (string arg in args)
				addArgument.Invoke(opt, new object[] { arg });
		}

		public object GetDriverObj(string browser, object opt)
		{
			string className = GetSeleniumClassName(browser, "Driver");
			//removed support for edge with selenium 3
			//if ( "edge" == browser.ToLower()) //a diferencia de java, hay que poner este atributo en las options
			//	opt.GetType().GetProperty("UseChromium").SetValue(opt, true);
			return Activator.CreateInstance(Type.GetType(className), new object[] { opt });
		}

		public object GetRemoteDriverObj(string remoteUrl, object opt)
		{
			string className = GetSeleniumClassName("remote", "Driver");
			return Activator.CreateInstance(Type.GetType(className), new object[] { new Uri(remoteUrl), opt });
		}

		private string GetSeleniumClassName(string browser, string clstype)
		{
			browser = browser.ToLower();
			string pkg = "OpenQA.Selenium." + Capitalize(browser);
			string assembly = "WebDriver";
			//Handle exceptions to the class name to be obtained
			string cls = "remote".Equals(browser) ? "RemoteWeb" : Capitalize(browser);
			//removed support for edge with selenium 3
			//if ("edge"==browser)
			//{
			//pkg = "Microsoft.Edge.SeleniumTools";
			//assembly = "Microsoft.Edge.SeleniumTools";
			//}
			//en net debe incluirse el nombre del assembly
			string className = pkg + "." + cls + clstype + ", " + assembly;
			log.Trace("Getting instance of class: " + className);
			return className;
		}
		private string Capitalize(string input)
		{
			return input.Substring(0, 1).ToUpper() + input.Substring(1);
		}

		private string GetDrivermanagerClassName(string browser)
		{
			string className = "WebDriverManager.DriverConfigs.Impl." + Capitalize(browser.ToLower()) + "Config" + ", WebDriverManager";
			log.Debug("Getting instance of class: " + className);
			return className;
		}
		private void SetUpWebDriver(IDriverConfig Config, string version)
		{
			//Executing several concurrent processes raises exceptions in some cases.
			//Perform retries to recover from this problem
			for (int i = 0; i < 20; i++)
			{
				try
				{
					// Jul 2023 removed match current browser, if chrome 115, selema try to find that version that does not exist
					// Using latest available driver gives a more consistent behaviour
					SetupWebDriverManager(Config, version);
					string driverPath = GetSeleniumDriverPath(Config, true);
					log.Debug("Downloaded web driver at: " + driverPath);
					return;
				}
				catch (IOException e)
				{
					log.Warn("Problem downloading driver, maybe it has been already downloaded but locked, retry...");
					log.Warn("Exception message: " + e.Message);
					System.Threading.Thread.Sleep(2000);
				}
			}
			//if here, the driver has not been downloaded after all repetitions
			throw new SelemaException("WebDriver download failed after 20 retries");
		}
        private void SetupWebDriverManager(IDriverConfig config, string driverVersion)
        {
            if (DriverVersion.MatchBrowser == driverVersion)
                new DriverManager().SetUpDriver(config, VersionResolveStrategy.MatchingBrowser);
            else if (DriverVersion.LatestAvailable == driverVersion)
                new DriverManager().SetUpDriver(config, VersionResolveStrategy.Latest);
            else if (DriverVersion.SeleniumManager == driverVersion)
                return;
            else // none of these keywords, try to get the exact version
                new DriverManager().SetUpDriver(config, driverVersion);
        }
        private string GetSeleniumDriverPath(WebDriverManager.DriverConfigs.IDriverConfig config, bool includeBinaryName)
		{
			string binaryName = includeBinaryName ? config.GetBinaryName() : "";
			return FileHelper.GetBinDestination(config.GetName(), config.GetLatestVersion(),
				ArchitectureHelper.GetArchitecture(), binaryName);
		}

		/// <summary>
		/// Downloads the driver executable for the specified browser using WebDriverManagerNet
		/// </summary>
		public void DownloadDriverExecutable(string browser, string version)
		{
			try
            {
				string clasName = GetDrivermanagerClassName(browser);
				object browserConfig = Activator.CreateInstance(Type.GetType(clasName));
				//Fix firefox can't get version matching browser
				if (browser == "firefox" && version==DriverVersion.MatchBrowser)
				{
					log.Warn("WebDriverManager.Net can not match Firefox driver and browser versions, using latest driver version");
					version = DriverVersion.LatestAvailable;
				}
				//second parametr indicates that browser version will be used, available only for chrome
				SetUpWebDriver((IDriverConfig)browserConfig, version);
			}
			catch (Exception e)
            {
				throw new SelemaException(log, "Can't download driver executable for browser: " + browser, e);
			}
		}

     }
}
