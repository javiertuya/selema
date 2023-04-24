package test4giis.selema.core;

import static org.junit.Assert.assertEquals;

import java.util.Map;
import java.util.TreeMap;

import org.junit.Test;
import org.openqa.selenium.UnexpectedAlertBehaviour;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeOptions;

import giis.selema.manager.SeleniumDriverFactory;

/**
 * This test is java only, contains many dependencies that are difficult to be translated
 */
public class TestDriverPlatformDependent {

	@Test
	public void testHeadlessWebDriverWithOptionsAndOptionsInstance() {
		if (!TestDriver.useHeadless()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		Map<String, Object> caps=new TreeMap<String, Object>();
		caps.put("testprefix:key1", "value1");
		ChromeOptions optInstance=new ChromeOptions();
		optInstance.setUnhandledPromptBehaviour(UnexpectedAlertBehaviour.IGNORE);
		WebDriver driver=factory.getSeleniumDriver("chrome", "", caps, TestDriver.chromeHeadlesArgument, optInstance);
		assertEquals("Capabilities {browserName: chrome, goog:chromeOptions: {args: [--headless, --remote-allow-origins=*], extensions: []}, testprefix:key1: value1, unhandledPromptBehavior: ignore}",
				factory.getLastOptionString());
		driver.close();
	}

}
