using OpenQA.Selenium;
using Giis.Portable.Util;
using Giis.Selema.Portable.Selenium;
using Giis.Selema.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services.Impl
{
    /// <summary>
    /// Management of watermarks that are placed in the web page under test
    /// </summary>
    public class WatermarkService : IWatermarkService
    {
        private string elementId;
        private int delay;
        private string backgroundColor;
        public WatermarkService()
        {
            SetElementId("selema-watermark");
            SetDelayOnFailure(1);
        }

        /// <summary>
        /// Sets the name of de web element id that will store the watermarks
        /// </summary>
        public virtual IWatermarkService SetElementId(string waternarkElementId)
        {
            this.elementId = waternarkElementId;
            return this;
        }

        /// <summary>
        /// After a test failure, waits for the specified time in seconds to give more time to watch the state of the browser (interactively or in a video)
        /// </summary>
        public virtual IWatermarkService SetDelayOnFailure(int delayOnFailure)
        {
            this.delay = delayOnFailure;
            return this;
        }

        /// <summary>
        /// Sets a background color to better differentiate the watermark from the web content (by default watermark has no background)
        /// </summary>
        public virtual IWatermarkService SetBackground(string color)
        {
            this.backgroundColor = color;
            return this;
        }

        /// <summary>
        /// Inserts a normal watermark (green)
        /// </summary>
        public virtual void Write(IWebDriver driver, string value)
        {
            WriteToBrowser(driver, value, "", "darkgreen");
        }

        /// <summary>
        /// Inserts a failure watermark (red)
        /// </summary>
        public virtual void Fail(IWebDriver driver, string value)
        {
            WriteToBrowser(driver, value, "FAIL ", "red");
            for (int i = 0; i < delay; i++)
            {
                JavaCs.Sleep(1000);
                value += " ."; //NOSONAR
                WriteToBrowser(driver, value, "FAIL ", "red");
            }
        }

        // Creates the element to hold the watermar (if does not exists) and writes the text
        private void WriteToBrowser(IWebDriver driver, string value, string prefix, string color)
        {
            string js = "var elem=document.getElementById('" + this.elementId + "'); " + "if (elem==null) { " + "  var spn = document.createElement('span'); " + "  spn.setAttribute('id','" + this.elementId + "'); " + "  document.body.appendChild(spn); " + "  elem=spn; " + "} " + "if (elem!=null) { " + "  elem.style.position='absolute'; " + "  elem.style.left='" + "1px" + "'; " + "  elem.style.top='" + "1px" + "'; " + "  elem.style.fontSize='" + "small" + "'; " + (this.backgroundColor == null || "".Equals(backgroundColor) ? "" : "  elem.style.background='" + this.backgroundColor + "'; ") + "  elem.style.color='" + color + "'; " + "  elem.textContent='" + prefix + value + "'; " + "}";
            SeleniumActions.ExecuteScript(driver, js);
        }
    }
}