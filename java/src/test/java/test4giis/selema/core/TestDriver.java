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
import giis.selema.manager.SeleManager;
import giis.selema.portable.SelemaException;
import giis.selema.services.impl.SelenoidService;

/**
 * Main tests to check that drivers are created
 * for diferent browsers, modes, arguments and parameters
 */
public class TestDriver {
	//As of Chrome Driver V 111, we need to include remote-allow-origins argument, 
	//if not connection with driver fails
	private String[] chromeHeadlesArgument=new String[] {"--headless", "--remote-allow-origins=*"};
	
	//Not all tests can be executed in all test modes,
	//all in local plus remote driver in CI, headless in local
	private boolean isLocal() {
		return new CiServiceFactory().getCurrent().isLocal();
	}
	private boolean useRemote() {
		return new CiServiceFactory().getCurrent().isLocal() || new Config4test().useRemoteWebDriver();
	}
	private boolean useHeadless() {
		return new CiServiceFactory().getCurrent().isLocal() || new Config4test().useHeadlessDriver();
	}

	@Before
	public void setUp() {
	}
	@After
	public void tearDown() {
	}

	@Test
	public void testLocalWebDriverChrome() {
		if (!isLocal()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		WebDriver driver=factory.getSeleniumDriver("chrome", "", null, new String[] {"--remote-allow-origins=*"});
		assertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--remote-allow-origins=*]}}");
		driver.close();
	}
	@Test
	public void testLocalWebDriverEdge() {
		if (!isLocal()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		WebDriver driver=factory.getSeleniumDriver("edge", "", null, null);
		assertOptions(factory, new SelemaConfig().isJava()
				? "{browserName:MicrosoftEdge,ms:edgeOptions:{args:[]}}"
				: "{browserName:MicrosoftEdge,ms:edgeOptions:{}}");
		driver.close();
	}
	@Test
	public void testLocalWebDriverFirefox() {
		if (!isLocal()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		WebDriver driver=factory.getSeleniumDriver("firefox", "", null, null);
		assertOptions(factory, "{browserName:firefox,moz:firefoxOptions:{}}");
		driver.close();
	}
	
	//next tests use chrome

	@Test
	public void testHeadlessWebDriverDefault() {
		if (!useHeadless()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		WebDriver driver=factory.getSeleniumDriver("chrome", "", null, chromeHeadlesArgument);
		assertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}");
		driver.close();
		//browser name is case insensitive, browser already downloaded, null remote url
		driver=factory.getSeleniumDriver("CHRome", null, null, chromeHeadlesArgument);
		assertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}");
		driver.close();
	}
	
	@Test
	public void testHeadlessWebDriverWithOptions() {
		if (!useHeadless()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		Map<String, Object> caps=new TreeMap<String, Object>();
		caps.put("key1", "value1");
		caps.put("key2", "value2");
		WebDriver driver=factory.getSeleniumDriver("chrome", "", caps, chromeHeadlesArgument);
		assertOptions(factory, new SelemaConfig().isJava() //different order on net
			? "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]},key1:value1,key2:value2}"
			: "{browserName:chrome,key1:value1,key2:value2,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}");
		driver.close();
	}
	@Test
	public void testHeadlessWebDriverNotFound() {
		if (!useHeadless()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		try {
			factory.getSeleniumDriver("carome", "", null, chromeHeadlesArgument);
			fail("Should fail");
		} catch (SelemaException e) {
			assertTrue(e.getMessage().startsWith("Can't instantiate WebDriver Options for browser: carome"));
		}
	}
	@Test
	public void testHeadlessWebDriverUnloadable() {
		if (!useHeadless()) return;
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
		if (!useRemote()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		WebDriver driver=factory.getSeleniumDriver("chrome", new Config4test().getRemoteDriverUrl(), null, null);
		//NOTE: Selenium 4.8.2/3 (java) adds --remote-allow-origins=* to prevent the Chrome Driver 111 breaking change
		assertOptions(factory, new SelemaConfig().isJava()
				? "{browserName:chrome,goog:chromeOptions:{args:[--remote-allow-origins=*]}}"
				: "{browserName:chrome,goog:chromeOptions:{}}");
		driver.close();
	}
	@Test
	public void testRemoteWebDriverWithArguments() {
		if (!useRemote()) return;
		//setting options has been tested with local
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		WebDriver driver=factory.getSeleniumDriver("chrome", new Config4test().getRemoteDriverUrl(), null, new String[] {"--start-maximized"});
		//NOTE: Selenium 4.8.2/3 (java) adds --remote-allow-origins=* to prevent the Chrome Driver 111 breaking change
		assertOptions(factory, new SelemaConfig().isJava()
				? "{browserName:chrome,goog:chromeOptions:{args:[--remote-allow-origins=*,--start-maximized]}}"
				: "{browserName:chrome,goog:chromeOptions:{args:[--start-maximized]}}");
		driver.close();
	}
	@Test
	public void testRemoteWebDriverWrongUrl() {
		if (!useRemote()) return;
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
		if (!useRemote()) return;
		Map<String,Object> capsToAdd=new TreeMap<String,Object>();
		capsToAdd.put("key1","value1");
		capsToAdd.put("key2","value2");
		SeleManager sm=new SeleManager(Config4test.getConfig())
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
		if (!useRemote()) return;
		SeleManager sm=new SeleManager(Config4test.getConfig()).setDriverUrl(new Config4test().getRemoteDriverUrl()).add(new SelenoidService());
		sm.onSetUp("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
		sm.onFailure("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
		assertLogRemoteWebDriver();
		sm.onTearDown("TestDriver", "TestDriver.testRemoteWebDriverFromManager");
	}
	private void assertLogRemoteWebDriver() {
		LogReader logReader=new LifecycleAsserts().getLogReader();
		logReader.assertBegin();
		logReader.assertContains("Creating SeleManager instance");
		logReader.assertContains("*** SetUp - TestDriver.testRemoteWebDriverFromManager");
		logReader.assertContains("Remote session chrome starting");
		logReader.assertContains("Remote session chrome started. Remote web driver at "+new Config4test().getRemoteDriverUrl());
		logReader.assertContains("FAIL TestDriver.testRemoteWebDriverFromManager");
		logReader.assertContains("Taking screenshot", "TestDriver-testRemoteWebDriverFromManager.png");
		logReader.assertEnd();
	}

}
