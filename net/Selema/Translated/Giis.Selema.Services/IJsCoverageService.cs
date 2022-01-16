/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using OpenQA.Selenium;
using Sharpen;

namespace Giis.Selema.Services
{
	/// <summary>Additional support to measure javascript coverage</summary>
	public interface IJsCoverageService
	{
		/// <summary>Configures this service, called on attaching the service to a SeleniumManager</summary>
		IJsCoverageService Configure(ISelemaLogger thisLog, string reportDir);

		/// <summary>To be executed after driver creation</summary>
		void AfterCreateDriver(IWebDriver driver);

		/// <summary>To be executed before driver ends</summary>
		void BeforeQuitDriver(IWebDriver driver);
	}
}
