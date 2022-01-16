/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using OpenQA.Selenium;
using Sharpen;

namespace Giis.Selema.Services
{
	/// <summary>Screenshot management</summary>
	public interface IScreenshotService
	{
		/// <summary>Configures this service, called on attaching the service to a SeleniumManager</summary>
		IScreenshotService Configure(ISelemaLogger thisLog);

		/// <summary>Takes an picture of the current state of the browser and saves it to the reports folder</summary>
		string TakeScreenshot(IWebDriver driver, IMediaContext context, string testName);
	}
}
