using System;
using Giis.Portable.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Giis.Selema.Portable.Selenium
{
	/// <summary>
	/// Miscelaneous utilites to perform some common Selenium actions
	/// </summary>
	public class SeleniumActions
	{
		private const int DefaultTimeout = 10;
		private const int HalfTimeout = 5;
		private const int ZeroTimeout = 0;

		private SeleniumActions()
		{
			throw new InvalidOperationException("Utility class");
		}

		/// <summary>
		/// Stores a screenshot in a file
		/// </summary>
		public static void TakeScreenshotToFile(IWebDriver driver, string fileName)
		{
			Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
			ss.SaveAsFile(fileName, ScreenshotImageFormat.Png);
		}

		/// <summary>
		/// Click using Actions
		/// </summary>
		public static void ClickActions(IWebDriver driver, IWebElement elem)
		{
			Actions actions = new Actions(driver);
			actions.MoveToElement(elem).Click().Perform();
		}
		/// <summary>
		/// Click using the javascript executor
		/// </summary>
		public static void ClickJavascript(IWebDriver driver, IWebElement elem)
		{
			ExecuteScript(driver, "arguments[0].click();", elem);
		}
		/// <summary>
		/// SendKeys using Actions.
		/// </summary>
		/// <remarks>
		/// Si se usa driver.click() se pueden producir excepciones como "element is obscured"
		/// -En firefox funciona normalmente, se corresponde con esta implementacion
		/// -En chrome no funciona con algunos elementos de texto (p.e. los de handsontable) por lo que se usara una implementacion distinta.
		/// Ver p.e. al final de: http://stackoverflow.com/questions/31547459/selenium-sendkeys-different-behaviour-for-chrome-firefox-and-safari
		/// </remarks>
		public static void SendKeysActions(IWebDriver driver, IWebElement elem, string value)
		{
			Actions actions = new Actions(driver);
			actions.MoveToElement(elem).Click().SendKeys(value).Perform();
		}

		/// <summary>
		/// SendKeys using the javascript executor
		/// </summary>
		public static void SendKeysJavascript(IWebDriver driver, IWebElement elem, string value)
		{
			ExecuteScript(driver, "arguments[0].value = arguments[1];", elem, value);
		}

		public static string ExecuteScript(IWebDriver driver, string script)
		{
			return (string)((IJavaScriptExecutor)driver).ExecuteScript(script);
		}
		public static string ExecuteScript(IWebDriver driver, string script, IWebElement elem)
		{
			return (string)((IJavaScriptExecutor)driver).ExecuteScript(script, elem);
		}
		public static string ExecuteScript(IWebDriver driver, string script, IWebElement elem, string value)
		{
			return (string)((IJavaScriptExecutor)driver).ExecuteScript(script, elem, value);
		}

		public static void SetInnerHtml(IWebDriver driver, string id, string value)
		{
			ExecuteScript(driver, "document.getElementById('" + id + "').innerHTML = '" + value + "';");
		}
		public static void SetInnerText(IWebDriver driver, string id, string value)
		{
			ExecuteScript(driver, "document.getElementById('" + id + "').textContent = '" + value + "';");
		}

		public static WebDriverWait NewWebDriverWait(IWebDriver driver, int seconds)
		{
			return new WebDriverWait(driver, new TimeSpan(0, 0, seconds));
		}

		/// <summary>
		/// Locates an element by the given locator using a Wait until clickable
		/// (more conditions https://www.selenium.dev/selenium/docs/api/java/org/openqa/selenium/support/ui/ExpectedConditions.html)
		/// </summary>
		public static IWebElement FindBy(IWebDriver driver, By locator)
		{
			WebDriverWait wait = NewWebDriverWait(driver, DefaultTimeout);
			return wait.Until(ExpectedConditions.ElementToBeClickable(locator));
		}
		/// <summary>
		/// Locates an element by id using a Wait until clickable
		/// </summary>
		public static IWebElement FindById(IWebDriver driver, string id)
		{
			return FindBy(driver, By.Id(id));
		}

		/// <summary>
		/// Waits until the text value of an element contains expected
		/// </summary>
		public static void WaitUntilTextPresent(IWebDriver seleniumDriver, IWebElement elem, string expected)
		{
			WebDriverWait wait = NewWebDriverWait(seleniumDriver, DefaultTimeout);
			wait.Message=("Expected text not found in element.");
			//Requires SeleniumExtras
			wait.Until(ExpectedConditions.TextToBePresentInElement(elem, expected));
		}

		/// <summary>
		/// Manages the response to an open pop-up window with a single method: alert, confirm, prompt
		/// </summary>
		/// <param name="driver">the WebDriver</param>
		/// <param name="accept">if true, clicks accept</param>
		/// <param name="dismiss">if true, clicks cancel (no compatible with accept)</param>
		/// <param name="text">text to write to a prompt pop-up (null if no text)</param>
		/// <param name="waitForClosedAlert">if true, uses a webdriver wait until alert is closed (recommended)</param>
		/// <param name="waitTime">includes a sleep in milliseconds before performing any acction (0 no time)</param>
		/// <returns>returned by the pop-up</returns>
		public static string ManageAlert(IWebDriver driver, bool accept, bool dismiss, string text, bool waitForClosedAlert, int waitTime)
		{
			if (waitTime > 0)
				JavaCs.Sleep(waitTime);

			//wait for the alert
			WebDriverWait wait = NewWebDriverWait(driver, HalfTimeout);
			wait.Until(ExpectedConditions.AlertIsPresent());
			if (waitTime > 0)
				JavaCs.Sleep(waitTime);

			//actions
			IAlert alert = driver.SwitchTo().Alert();
			string alertText = alert.Text;
			if (text != null)
				alert.SendKeys(text);
			if (accept)
				alert.Accept();
			if (dismiss)
				alert.Dismiss();
			if (waitTime > 0)
				JavaCs.Sleep(waitTime);

			//spometimes alert contunes open while this method has finished, waits until alert is closed
			if (waitForClosedAlert)
				try
				{
					wait = NewWebDriverWait(driver, ZeroTimeout);
					wait.Until(ExpectedConditions.AlertIsPresent());
					JavaCs.Sleep(1000);
				}
				catch (Exception)
				{
					//expected behaviour, not present
				}
			return alertText;
		}

	}
}
