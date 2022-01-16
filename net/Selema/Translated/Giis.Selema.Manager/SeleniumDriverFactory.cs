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

		/// <summary>Obtiene el driver local adecuado al browser</summary>
		public virtual IWebDriver GetLocalSeleniumDriver(string browser, IDictionary<string, object> options, string[] arguments)
		{
			return GetSeleniumDriver(browser, null, options, arguments);
		}

		/// <summary>
		/// Obtiene el driver para ejecucion en un navegador a traves del RemoteWebDriver que se encuentra en la url especificada
		/// y con la configuracion requerida para uso de Selenoid
		/// </summary>
		public virtual RemoteWebDriver GetRemoteSeleniumDriver(string browser, string remoteUrl, IDictionary<string, object> options, string[] arguments)
		{
			return (RemoteWebDriver)GetSeleniumDriver(browser, remoteUrl, options, arguments);
		}

		/// <summary>Crea una instancia del driver correspondiente al browser indicado inicializando las options y arguments</summary>
		private IWebDriver GetSeleniumDriver(string browser, string remoteUrl, IDictionary<string, object> caps, string[] args)
		{
			SeleniumObjects reflect = new SeleniumObjects();
			try
			{
				//Establece las capabilities y argumentos en un objeto options
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
				//creacion del driver con las opciones
				log.Debug("Setting up WebDriver, browser: " + browser + ", url: " + remoteUrl);
				if (remoteUrl == null || string.Empty.Equals(remoteUrl.Trim()))
				{
					return (IWebDriver)reflect.GetDriverObj(browser, opt);
				}
				else
				{
					return (RemoteWebDriver)reflect.GetRemoteDriverObj(remoteUrl, opt);
				}
			}
			catch (Exception e)
			{
				//
				log.Error("Can't instantiate object from browser: " + browser + ". See debug log for more details");
				throw new SelemaException("Can't instantiate object for browser: " + browser + ". Message: " + e.Message);
			}
		}

		/// <summary>Descarga el driver adecuado al browser</summary>
		public virtual void DownloadLocalDriver(string browser)
		{
			new SeleniumObjects().DownloadDriverExecutable(browser);
		}
	}
}
