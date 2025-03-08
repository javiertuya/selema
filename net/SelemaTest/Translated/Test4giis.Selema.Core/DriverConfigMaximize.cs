using OpenQA.Selenium;
using Giis.Selema.Manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Core
{
    public class DriverConfigMaximize : IDriverConfigDelegate
    {
        public virtual void Configure(IWebDriver driver)
        {
            driver.Manage().Window.Maximize();
        }
    }
}