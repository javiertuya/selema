using OpenQA.Selenium;

namespace Giis.Selema.Portable.Selenium
{
    /// <summary>
    /// Utilities to facilitate Java to Net conversions of the
    /// interactions with the Selenium Driver
    /// </summary>
    public class DriverUtil
    {
        public static void GetUrl(IWebDriver driver, string url)
        {
            driver.Url = url;
        }
        public static void CloseDriver(IWebDriver driver)
        {
            driver.Close();
        }
        public static void QuitDriver(IWebDriver driver)
        {
            driver.Quit();
        }
        public static string GetTitle(IWebDriver driver)
        {
            return driver.Title;
        }

    }
}
