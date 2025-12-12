using System.Reflection;
using Java.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using NLog;
using Giis.Portable.Util;
using Giis.Selema.Portable.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Manager
{
    /// <summary>
    /// Instanciacion de driver local y remoto ado el nombre del navegador
    /// Aplicable a los drivers para los navegadores de nombre NAME (puede ser minusculas)
    /// donde la clase de selenium sigue el patron: org.openqa.selenium.name.NameDriver;
    /// </summary>
    public class SeleniumDriverFactory
    {
        readonly Logger log = LogManager.GetCurrentClassLogger();
        private string lastOptionString = "";
        /// <summary>
        /// Unified entry point to instantiate a WebDriver for the indicated browser adding the capabilities and arguments specified;
        /// if the remoteUrl is empty or null returns a WebDriver (downloading the driver executable if needed),
        /// if not, returns a RemoteWebDriver
        /// </summary>
        public virtual WebDriver GetSeleniumDriver(string browser, string remoteUrl, string driverVersion, Map<string, object> caps, string[] args, DriverOptions optInstance)
        {
            SeleniumObjects reflect = new SeleniumObjects();
            string objectToInstantiate = ""; //to enhance error messages
            string url = "";
            try
            {
                objectToInstantiate = "WebDriver Options";

                //Sets capabilities and arguments by create an options object
                log.Debug("Setting up WebDriver Options, browser: " + browser);
                object opt = optInstance == null ? reflect.GetOptionsObj(browser, args) : optInstance;
                if (caps != null)
                    foreach (string key in caps.KeySet())
                        reflect.SetCapability(opt, key, caps[key]);
                if (args != null)
                    reflect.AddArguments(opt, args);

                //Creates either local or remote web driver
                objectToInstantiate = "WebDriver";
                log.Debug("Setting up WebDriver, browser: " + browser + ", url: " + MaskUrl(remoteUrl));
                lastOptionString = reflect.GetOptionsObjAsString(opt);
                log.Trace("Option string: " + lastOptionString.Replace("\n", "").Replace("\r", ""));
                if (remoteUrl == null || "".Equals(remoteUrl.Trim()))
                {
                    EnsureLocalDriverDownloaded(browser, driverVersion);
                    return (WebDriver)reflect.GetDriverObj(browser, opt);
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

                //NOSONAR
                string message = "Can't instantiate " + objectToInstantiate + " for browser: " + browser + ("".Equals(url) ? "" : " at url: " + MaskUrl(url));
                if (e is TargetInvocationException)
                    message += ". Message: " + ((TargetInvocationException)e).InnerException.Message;
                message += ". Exception: " + e.GetType().FullName; //add exception class name for better debugging
                throw new SelemaException(log, message, e);
            }
        }

        /// <summary>
        /// Gets the Options object (as string) that corresponds to the latest driver instantiated (only for testing purposes)
        /// </summary>
        public virtual string GetLastOptionString()
        {
            return lastOptionString;
        }

        /// <summary>
        /// Ensures that the appropriate local driver has been downladed,
        /// </summary>
        public virtual void EnsureLocalDriverDownloaded(string browser, string version)
        {

            // sanitize inputs before download
            browser = browser.ToLower();
            version = version == null || "".Equals(version.Trim()) ? DriverVersion.DEFAULT : version.ToLower();
            new SeleniumObjects().DownloadDriverExecutable(browser, version);
        }

        /// <summary>
        /// Mask the password in a webdriver url that contains username and password for grid authentication
        /// </summary>
        public static string MaskUrl(string url)
        {
            if (url == null)
                return url;
            int start = url.IndexOf("://");
            int end = url.IndexOf("@");
            if (start < 0 || end < 0 || start >= end)
                return url;
            start += 3;
            string userInfoStr = JavaCs.Substring(url, start, end);
            string[] userInfo = JavaCs.SplitByChar(userInfoStr, ':');
            if (userInfo.Length > 1 || userInfoStr.EndsWith(":"))
                return JavaCs.Substring(url, 0, start) + userInfo[0] + ":" + "******" + JavaCs.Substring(url, end, url.Length);
            else
                return url;
        }
    }
}