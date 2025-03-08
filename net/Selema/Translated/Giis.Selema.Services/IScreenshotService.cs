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
    /// Screenshot management
    /// </summary>
    public interface IScreenshotService
    {
        /// <summary>
        /// Configures this service, called on attaching the service to a SeleManager
        /// </summary>
        IScreenshotService Configure(ISelemaLogger thisLog);
        /// <summary>
        /// Takes an picture of the current state of the browser and saves it to the reports folder
        /// </summary>
        string TakeScreenshot(IWebDriver driver, IMediaContext context, string testName);
    }
}