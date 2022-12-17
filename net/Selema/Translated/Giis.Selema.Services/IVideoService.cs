/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using OpenQA.Selenium;
using Sharpen;

namespace Giis.Selema.Services
{
	/// <summary>Support for video recording from a browser service</summary>
	public interface IVideoService
	{
		/// <summary>Configures this service, called on attaching the service to a SeleManager</summary>
		IVideoService Configure(ISelemaLogger thisLog);

		void BeforeCreateDriver();

		void AfterCreateDriver(IWebDriver driver);

		string OnTestFailure(IMediaContext context, string testName);

		/// <summary>Gets the capabilities that the WebDriver must configure to integrate with this service</summary>
		IDictionary<string, object> GetSeleniumOptions(IMediaContext context, string testName);

		void BeforeQuitDriver(IMediaContext context, string testName);
	}
}
