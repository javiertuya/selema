/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Manager;
using OpenQA.Selenium;
using Sharpen;

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
