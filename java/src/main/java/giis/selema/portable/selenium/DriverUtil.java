package giis.selema.portable.selenium;

import org.openqa.selenium.WebDriver;

/**
 * Utilities to facilitate Java to Net conversions of the
 * interactions with the Selenium Driver
 */
public class DriverUtil {
	private DriverUtil() {
	    throw new IllegalAccessError("Utility class");
	}
	public static void getUrl(WebDriver driver, String url) {
		driver.get(url);
	}
	
	public static void closeDriver(WebDriver driver) {
		driver.close();
	}
	
	public static String getTitle(WebDriver driver) {
		return driver.getTitle();
	}

}