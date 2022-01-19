using NLog;
using System;
using System.IO;
using System.Reflection;
using WebDriverManager;
using WebDriverManager.DriverConfigs;
using WebDriverManager.Helpers;

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

		public void SetCapability(object opt, string key, object value)
		{
			//Selenium 3 difference from java: to add a capability a thir parameter is required (true: global)
			//In selenium 4 AddAdditionalCapability is deprecated, AddAdditionalOption should be used
			//Uses the deprecated one to keep compatibility with selenium 3
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
			log.Debug("Getting instance of class: " + className);
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
		private void SetUpWebDriver(IDriverConfig Config, bool matchCurrentBrowser)
		{
			//Executing several concurrent processes raises exceptions in some cases.
			//Perform retries to recover from this problem
			for (int i = 0; i < 20; i++)
			{
				try
				{
					new DriverManager().SetUpDriver(Config, matchCurrentBrowser ? VersionResolveStrategy.MatchingBrowser : VersionResolveStrategy.Latest);
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
		private string GetSeleniumDriverPath(WebDriverManager.DriverConfigs.IDriverConfig config, bool includeBinaryName)
		{
			string binaryName = includeBinaryName ? config.GetBinaryName() : "";
			return FileHelper.GetBinDestination(config.GetName(), config.GetLatestVersion(),
				ArchitectureHelper.GetArchitecture(), binaryName);
		}

		/// <summary>
		/// Downloads the driver executable for the specified browser using WebDriverManagerNet
		/// </summary>
		public void DownloadDriverExecutable(string browser)
		{
			try
            {
				string clasName = GetDrivermanagerClassName(browser);
				object browserConfig = Activator.CreateInstance(Type.GetType(clasName));
				//second parametr indicates that browser version will be used, available only for chrome
				SetUpWebDriver((IDriverConfig)browserConfig, browser.ToLower() == "chrome");
			}
			catch (Exception e)
            {
				throw new SelemaException(log, "Can't download driver executable for browser: " + browser, e);
			}
		}

	}
}
