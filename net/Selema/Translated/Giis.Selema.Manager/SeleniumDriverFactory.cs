/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using Giis.Selema.Portable;
using Giis.Selema.Portable.Selenium;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Sharpen;

namespace Giis.Selema.Manager
{
	/// <summary>
	/// Instanciacion de driver local y remoto ado el nombre del navegador
	/// Aplicable a los drivers para los navegadores de nombre NAME (puede ser minusculas)
	/// donde la clase de selenium sigue el patron: org.openqa.selenium.name.NameDriver;
	/// </summary>
	public class SeleniumDriverFactory
	{
		internal readonly Logger log = LogManager.GetCurrentClassLogger();

		private IList<string> driversWithSetupDone = new List<string>();

		private string lastOptionString = string.Empty;

		//to avoid duplicate downloads of local drivers
		/// <summary>
		/// Unified entry point to instantiate a WebDriver for the indicated browser adding the capabilities and arguments specified;
		/// if the remoteUrl is empty or null returns a WebDriver (downloading the driver executable if needed),
		/// if not, returns a RemoteWebDriver
		/// </summary>
		public virtual IWebDriver GetSeleniumDriver(string browser, string remoteUrl, IDictionary<string, object> caps, string[] args)
		{
			SeleniumObjects reflect = new SeleniumObjects();
			string objectToInstantiate = string.Empty;
			//to enhance error messages
			string url = string.Empty;
			try
			{
				objectToInstantiate = "WebDriver Options";
				//Sets capabilities and arguments by create an options object
				log.Debug("Setting up WebDriver Options, browser: " + browser);
				object opt = reflect.GetOptionsObj(browser, args);
				if (caps != null)
				{
					foreach (string key in caps.Keys)
					{
						// compatibility with .NET
						reflect.SetCapability(opt, key, caps[key]);
					}
				}
				if (args != null)
				{
					reflect.AddArguments(opt, args);
				}
				//Creates either local or remote web driver
				objectToInstantiate = "WebDriver";
				log.Debug("Setting up WebDriver, browser: " + browser + ", url: " + remoteUrl);
				lastOptionString = opt.ToString();
				log.Trace("Option string: " + lastOptionString);
				if (remoteUrl == null || string.Empty.Equals(remoteUrl.Trim()))
				{
					EnsureLocalDriverDownloaded(browser);
					return (IWebDriver)reflect.GetDriverObj(browser, opt);
				}
				else
				{
					objectToInstantiate = "RemoteWebDriver";
					url = remoteUrl;
					return (RemoteWebDriver)reflect.GetRemoteDriverObj(remoteUrl, opt);
				}
			}
			catch (Exception e)
			{
				//
				throw new SelemaException(log, "Can't instantiate " + objectToInstantiate + " for browser: " + browser + (string.Empty.Equals(url) ? string.Empty : " at url: " + url), e);
			}
		}

		/// <summary>Gets the Options object (as string) that corresponds to the latest driver instantiated (only for testing purposes)</summary>
		public virtual string GetLastOptionString()
		{
			return lastOptionString;
		}

		/// <summary>Ensures that the appropriate local driver has been downladed,</summary>
		public virtual void EnsureLocalDriverDownloaded(string browser)
		{
			browser = browser.ToLower();
			if (!driversWithSetupDone.Contains(browser))
			{
				new SeleniumObjects().DownloadDriverExecutable(browser);
				driversWithSetupDone.Add(browser);
			}
		}
	}
}
