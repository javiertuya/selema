package test4giis.selema.core;

import static org.junit.Assert.assertEquals;

import org.junit.Test;
import org.openqa.selenium.UnexpectedAlertBehaviour;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeOptions;

import giis.selema.manager.SeleniumDriverFactory;
import giis.selema.portable.selenium.DriverUtil;
import giis.selema.portable.selenium.JavaMap;

/**
 * This test is java only, contains many dependencies that are difficult to be translated
 */
public class TestDriverPlatformDependent {

	@Test
	public void testHeadlessWebDriverWithOptionsAndOptionsInstance() {
		if (!TestDriver.useHeadless()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		JavaMap<String, Object> caps = new JavaMap<String, Object>(true);
		caps.put("testprefix:key1", "value1");
		ChromeOptions optInstance=new ChromeOptions();
		optInstance.setUnhandledPromptBehaviour(UnexpectedAlertBehaviour.IGNORE);
		WebDriver driver=factory.getSeleniumDriver("chrome", "", "", caps.unwrap(), TestDriver.chromeHeadlesArgument, optInstance);
		assertEquals("Capabilities {browserName: chrome, goog:chromeOptions: {args: [--headless, --remote-allow-origins=*], extensions: []}, testprefix:key1: value1, unhandledPromptBehavior: ignore}",
				factory.getLastOptionString());
		DriverUtil.closeDriver(driver);
	}

}
