package test4giis.selema.core;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertThrows;

import java.util.Map;
import java.util.TreeMap;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.openqa.selenium.WebDriver;

import giis.portable.util.Parameters;
import giis.selema.manager.CiServiceFactory;
import giis.selema.manager.SeleManager;
import giis.selema.manager.SelemaException;
import giis.selema.manager.SeleniumDriverFactory;
import giis.selema.portable.selenium.DriverUtil;
import giis.selema.services.impl.SeleniumGridService;
import giis.selema.services.impl.SelenoidService;
import test4giis.selema.portable.Asserts;

/**
 * Main tests to check that drivers are created
 * for diferent browsers, modes, arguments and parameters
 */
public class TestDriver {
	//As of Chrome Driver V 111, we need to include remote-allow-origins argument, 
	//if not connection with driver fails
	public static String[] chromeHeadlesArgument=new String[] {"--headless", "--remote-allow-origins=*"};
	
	//Not all tests can be executed in all test modes,
	//all in local plus remote driver in CI, headless in local
	public static boolean isLocal() {
		return new CiServiceFactory().getCurrent().isLocal();
	}
	public static boolean useRemote() {
		return new CiServiceFactory().getCurrent().isLocal() || new Config4test().useRemoteWebDriver();
	}
	public static boolean useHeadless() {
		return new CiServiceFactory().getCurrent().isLocal() || new Config4test().useHeadlessDriver();
	}

	WebDriver driver;
	
	@Before
	public void setUp() {
	}
	@After
	public void tearDown() {
		if (driver != null)
			DriverUtil.quitDriver(driver);
		driver = null;
	}

	@Test
	public void testLocalWebDriverChrome() {
		if (!isLocal()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		driver=factory.getSeleniumDriver("chrome", "", "", null, new String[] {"--remote-allow-origins=*"}, null);
		assertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--remote-allow-origins=*]}}");
	}
	@Test
	public void testLocalWebDriverEdge() {
		if (!isLocal()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		driver=factory.getSeleniumDriver("edge", "", "", null, null, null);
		assertOptions(factory, Parameters.isJava()
				? "{browserName:MicrosoftEdge,ms:edgeOptions:{args:[]}}"
				: "{browserName:MicrosoftEdge,ms:edgeOptions:{}}");
	}
	@Test
	public void testLocalWebDriverFirefox() {
		if (!isLocal()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		driver=factory.getSeleniumDriver("firefox", "", "", null, null, null);
		// #801 no toString method implemented for firefox
		assertOptions(factory, Parameters.isJava()
				? "{browserName:firefox,moz:firefoxOptions:{prefs:{remote.active-protocols:3}}}"
				: "OpenQA.Selenium.Firefox.FirefoxOptions");
	}
	
	//next tests use chrome

	@Test
	public void testHeadlessWebDriverDefault() {
		if (!useHeadless()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		driver=factory.getSeleniumDriver("chrome", "", "", null, chromeHeadlesArgument, null);
		assertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}");
	}
	
	@Test
	public void testHeadlessWebDriverDefaultCaseInsensitive() {
		if (!useHeadless()) return;
		//browser name is case insensitive
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		driver=factory.getSeleniumDriver("CHRome", null, null, null, chromeHeadlesArgument, null);
		assertOptions(factory, "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}");
	}
	
	@Test
	public void testHeadlessWebDriverWithOptions() {
		if (!useHeadless()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		Map<String, Object> caps = new TreeMap<String, Object>();
		//As of Selenium 4.9.0 non standard capabilities must have a prefix
		caps.put("testprefix:key1", "value1");
		caps.put("testprefix:key2", "value2");
		driver=factory.getSeleniumDriver("chrome", "", "", caps, chromeHeadlesArgument, null);
		// #801 the toString method is not able to get other custom capabilities than the standard and chromeOptions
		assertOptions(factory, Parameters.isJava() //different order on net
			? "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]},testprefix:key1:value1,testprefix:key2:value2}"
			: "{browserName:chrome,goog:chromeOptions:{args:[--headless,--remote-allow-origins=*]}}");
	}
	
	//testHeadlessWebDriverWithOptionsAndOptionsInstance tested in separate class (transformed manually to C#)
	
	@Test
	public void testHeadlessWebDriverNotFound() {
		if (!useHeadless()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		RuntimeException e = assertThrows(SelemaException.class, () -> {
			factory.getSeleniumDriver("carome", "", "", null, chromeHeadlesArgument, null);
		});
		Asserts.assertIsTrue(e.getMessage().startsWith("Can't instantiate WebDriver Options for browser: carome".replace("IWeb", "Web")),
				"Not contained in: " + e.getMessage());
	}
	@Test
	public void testHeadlessWebDriverUnloadable() {
		if (!useHeadless()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		RuntimeException e = assertThrows(SelemaException.class, () -> {
			factory.ensureLocalDriverDownloaded("corome", "");
		});
		Asserts.assertIsTrue(e.getMessage().startsWith("Can't download driver executable for browser: corome"),
				"Not contained in: " + e.getMessage());
	}
	//Custom assertion to allow same comparisons in java and net
	private void assertOptions(SeleniumDriverFactory factory, String expected) {
		expected=expected.replace(" ", "");
		String actual=factory.getLastOptionString().replace(" ", "");
		if (Parameters.isJava())
			actual=actual.replace("Capabilities{", "{").replace(",extensions:[]", "")
					.replace("acceptInsecureCerts:true,", "").replace("moz:debuggerAddress:true,", "");
		if (Parameters.isNetCore())
			actual=actual.replace("\n", "").replace("\r", "").replace("\"", "");
		assertEquals(expected, actual);
	}
	
	@Test
	public void testRemoteWebDriverDefault() {
		if (!useRemote()) return;
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		driver=factory.getSeleniumDriver("chrome", new Config4test().getRemoteDriverUrl(), null, null, null, null);
		//NOTE: Selenium 4.8.2/3 (java) adds --remote-allow-origins=* to prevent the Chrome Driver 111 breaking change
		//As of Selenium 4.14.1 remote-allow-origins is removed
		assertOptions(factory, Parameters.isJava()
				? "{browserName:chrome,goog:chromeOptions:{args:[]}}"
				: "{browserName:chrome,goog:chromeOptions:{}}");
	}
	@Test
	public void testRemoteWebDriverWithArguments() {
		if (!useRemote()) return;
		//setting options has been tested with local
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		driver=factory.getSeleniumDriver("chrome", new Config4test().getRemoteDriverUrl(), null, null, new String[] {"--start-maximized"}, null);
		//NOTE: Selenium 4.8.2/3 (java) adds --remote-allow-origins=* to prevent the Chrome Driver 111 breaking change
		//As of Selenium 4.14.1 remote-allow-origins is removed
		assertOptions(factory, Parameters.isJava()
				? "{browserName:chrome,goog:chromeOptions:{args:[--start-maximized]}}"
				: "{browserName:chrome,goog:chromeOptions:{args:[--start-maximized]}}");
	}
	@Test
	public void testRemoteWebDriverWrongUrl() {
		if (!useRemote()) return;
		String wrongUrl=new Config4test().getRemoteDriverUrl() + "/notexist";
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		RuntimeException e = assertThrows(SelemaException.class, () -> {
			factory.getSeleniumDriver("chrome", wrongUrl, null, null, null, null);
		});
		Asserts.assertIsTrue(
				e.getMessage().startsWith("Can't instantiate RemoteWebDriver for browser: chrome at url: " + wrongUrl),
				"Not contained in: " + e.getMessage());
	}

	//lifecycle tests with remote driver use a browser service, but it should work if not browser service is attached
	//As this is not inside a lifecycle controller, simulates the steps.
	@Test
	public void testRemoteWebDriverFromManagerNoBrowserService() {
		if (!useRemote()) return;
		Map<String,Object> capsToAdd = new TreeMap<String,Object>();
		capsToAdd.put("testprefix:key1","value1");
		capsToAdd.put("testprefix:key2","value2");
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
	public void testRemoteWebDriverFromManagerNoVideoWithCapability() {
		if (!useRemote()) return;
		SeleManager sm=new SeleManager(Config4test.getConfig())
				.setDriverUrl(new Config4test().getRemoteDriverUrl());
		// Browser service capabilities should be also included, in selenoid grouped under selenoid:options
		if (new Config4test().useSelenoidRemoteWebDriver())
			sm.add(new SelenoidService().setCapability("enableLog", true));
		else if (new Config4test().useSeleniumRemoteWebDriver())
			sm.add(new SeleniumGridService().setCapability("se:screenResolution", "800x600"));
		else
			return;
			//fail("This test should execute only with remote web driver");
		
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
