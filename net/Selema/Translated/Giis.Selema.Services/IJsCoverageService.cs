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
    /// Additional support to measure javascript coverage
    /// </summary>
    public interface IJsCoverageService
    {
        /// <summary>
        /// Configures this service, called on attaching the service to a SeleManager
        /// </summary>
        IJsCoverageService Configure(ISelemaLogger thisLog, string reportDir);
        /// <summary>
        /// To be executed after driver creation
        /// </summary>
        void AfterCreateDriver(IWebDriver driver);
        /// <summary>
        /// To be executed before driver ends
        /// </summary>
        void BeforeQuitDriver(IWebDriver driver);
    }
}