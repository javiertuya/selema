using Java.Util;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services
{
    /// <summary>
    /// Support for video recording from a browser service
    /// </summary>
    public interface IVideoService
    {
        /// <summary>
        /// Configures this service, called on attaching the service to a SeleManager
        /// </summary>
        IVideoService Configure(ISelemaLogger thisLog);
        void BeforeCreateDriver();
        void AfterCreateDriver(IWebDriver driver);
        string OnTestFailure(IMediaContext context, string testName);
        /// <summary>
        /// Gets the capabilities that the IWebDriver must configure to integrate with this service
        /// </summary>
        Map<string, object> GetSeleniumOptions(IMediaContext context, string testName);
        void BeforeQuitDriver(IMediaContext context, string testName);
    }
}