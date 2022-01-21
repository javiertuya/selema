package test4giis.selema.core;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;
import static org.junit.Assert.fail;

import java.util.Map;
import java.util.TreeMap;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.openqa.selenium.WebDriver;

import giis.selema.framework.junit4.Asserts;
import giis.selema.manager.CiServiceFactory;
import giis.selema.manager.SelemaConfig;
import giis.selema.manager.SeleniumDriverFactory;
import giis.selema.manager.SeleniumManager;
import giis.selema.portable.SelemaException;
import giis.selema.services.impl.SelenoidService;

/**
 * Some detailed tests for the driver instantiation features
 */
public class TestDriver {
	private String[] headlesArgument=new String[] {"--headless"};
	
	//Not all tests can be executed in all test modes,
	//all in local, remote driver on selenoid, local driver on headless
	private boolean onRemote() {
		return new CiServiceFactory().getCurrent().isLocal() || new Config4test().useRemoteWebDriver();
	}
	private boolean onLocal() {
		return new CiServiceFactory().getCurrent().isLocal() || new Config4test().useHeadlessDriver();
	}
	@Before
	public void setUp() {
	}
	@After
	public void tearDown() {
	}

	@Test
	public void testLocalWebDriverDefault() {
		if (!onLocal()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		WebDriver driver=factory.getSeleniumDriver("chrome", "", null, headlesArgument);
		assertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--headless]}}");
		driver.close();
		driver=factory.getSeleniumDriver("firefox", "", null, headlesArgument);
		assertOptions(factory, "{browserName:firefox,moz:firefoxOptions:{args:[--headless]}}");
		driver.close();
		//browser name is case insensitive, browser already downloaded, null remote url
		driver=factory.getSeleniumDriver("CHRome", null, null, headlesArgument);
		assertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--headless]}}");
		driver.close();
	}
	@Test
	public void testLocalWebDriverWithOptions() {
		if (!onLocal()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		Map<String, Object> caps=new TreeMap<String, Object>();
		caps.put("key1", "value1");
		caps.put("key2", "value2");
		WebDriver driver=factory.getSeleniumDriver("chrome", "", caps, headlesArgument);
		assertOptions(factory, new SelemaConfig().isJava() //different order on net
			? "{browserName:chrome,goog:chromeOptions:{args:[--headless]},key1:value1,key2:value2}"
			: "{browserName:chrome,key1:value1,key2:value2,goog:chromeOptions:{args:[--headless]}}");
		driver.close();
	}
	@Test
	public void testLocalWebDriverNotFound() {
		if (!onLocal()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		try {
			factory.getSeleniumDriver("carome", "", null, headlesArgument);
			fail("Should fail");
		} catch (SelemaException e) {
			assertTrue(e.getMessage().startsWith("Can't instantiate WebDriver Options for browser: carome"));
		}
	}
	@Test
	public void testLocalWebDriverUnloadable() {
		if (!onLocal()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		try {
			factory.ensureLocalDriverDownloaded("corome");
			fail("Should fail");
		} catch (SelemaException e) {
			assertTrue(e.getMessage().startsWith("Can't download driver executable for browser: corome"));
		}
	}
	//Custom assertion to allow same comparisons in java and net
	private void assertOptions(SeleniumDriverFactory factory, String expected) {
		SelemaConfig conf=new SelemaConfig();
		expected=expected.replace(" ", "");
		String actual=factory.getLastOptionString().replace(" ", "");
		if (conf.isJava())
			actual=actual.replace("Capabilities{", "{").replace(",extensions:[]", "")
					.replace("acceptInsecureCerts:true,", "").replace("moz:debuggerAddress:true,", "");
		if (conf.isNet())
			actual=actual.replace("\n", "").replace("\r", "").replace("\"", "");
		assertEquals(expected, actual);
	}
	
	@Test
	public void testRemoteWebDriverDefault() {
		if (!onRemote()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		WebDriver driver=factory.getSeleniumDriver("chrome", new Config4test().getRemoteDriverUrl(), null, null);
		assertOptions(factory, new SelemaConfig().isJava()
				? "{browserName:chrome,goog:chromeOptions:{args:[]}}"
				: "{browserName:chrome,goog:chromeOptions:{}}");
		driver.close();
	}
	@Test
	public void testRemoteWebDriverWithArguments() {
		if (!onRemote()) return;
		//setting options has been tested with local
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		WebDriver driver=factory.getSeleniumDriver("chrome", new Config4test().getRemoteDriverUrl(), null, new String[] {"--start-maximized"});
		assertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--start-maximized]}}");
		driver.close();
	}
	@Test
	public void testRemoteWebDriverWrongUrl() {
		if (!onRemote()) return;
		String wrongUrl=new Config4test().getRemoteDriverUrl() + "/notexist";
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		try {
			factory.getSeleniumDriver("chrome", wrongUrl, null, null);
			fail("Should fail");
		} catch (SelemaException e) {
			Asserts.assertIsTrue(
					e.getMessage().startsWith("Can't instantiate RemoteWebDriver for browser: chrome at url: " + wrongUrl),
					"Not contained in: " + e.getMessage());
		}
	}

	//lifecycle tests with remote driver use a browser service, but it should work if not browser service is attached
	//As this is not inside a lifecycle controller, simulates the steps.
	@Test
	public void testRemoteWebDriverFromManagerNoBrowserService() {
		if (!onRemote()) return;
		Map<String,Object> capsToAdd=new TreeMap<String,Object>();
		capsToAdd.put("key1","value1");
		capsToAdd.put("key2","value2");
		SeleniumManager sm=new SeleniumManager(Config4test.getConfig())
				.setDriverUrl(new Config4test().getRemoteDriverUrl())
				.setOptions(capsToAdd); //can't get options from driver instance, check at the debug log
		sm.onSetUp("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
		sm.onFailure("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
		assertLogRemoteWebDriver();
		sm.onTearDown("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
	}
	//with browser service but no video too
	@Test
	public void testRemoteWebDriverFromManagerNoVideoService() {
		if (!onRemote()) return;
		SeleniumManager sm=new SeleniumManager(Config4test.getConfig()).setDriverUrl(new Config4test().getRemoteDriverUrl()).add(new SelenoidService());
		sm.onSetUp("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
		sm.onFailure("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
		assertLogRemoteWebDriver();
		sm.onTearDown("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
	}
	private void assertLogRemoteWebDriver() {
		LogReader logReader=new LifecycleAsserts().getLogReader();
		logReader.assertBegin();
		logReader.assertContains("Creating SeleniumManager instance");
		logReader.assertContains("*** SetUp - TestDriver.testRemoteWebDriverFromManager");
		logReader.assertContains("Remote session chrome starting");
		logReader.assertContains("Remote session chrome started. Remote web driver at "+new Config4test().getRemoteDriverUrl());
		logReader.assertContains("FAIL TestDriver.testRemoteWebDriverFromManager");
		logReader.assertContains("Taking screenshot", "TestDriver-testRemoteWebDriverFromManager.png");
		logReader.assertEnd();
	}

}
