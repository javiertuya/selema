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
    /// Management of watermarks that are placed in the web page under test
    /// </summary>
    public interface IWatermarkService
    {
        /// <summary>
        /// Sets the name of de web element id that will store the watermarks
        /// </summary>
        IWatermarkService SetElementId(string waternarkElementId);
        /// <summary>
        /// After a test failure, waits for the specified time in seconds to give more time to watch the state of the browser (interactively or in a video)
        /// </summary>
        IWatermarkService SetDelayOnFailure(int delayOnFailure);
        /// <summary>
        /// Inserts a normal watermark (green)
        /// </summary>
        void Write(IWebDriver driver, string value);
        /// <summary>
        /// Inserts a failure watermark (red)
        /// </summary>
        void Fail(IWebDriver driver, string value);
        /// <summary>
        /// Sets a background color to better differentiate the watermark from the web content (by default watermark has no background)
        /// </summary>
        IWatermarkService SetBackground(string color);
    }
}