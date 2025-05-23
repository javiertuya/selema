package test4giis.selema.core;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertThrows;

import org.junit.After;
import org.junit.Before;
import org.junit.ClassRule;
import org.junit.Rule;
import org.junit.Test;
import org.openqa.selenium.WebDriver;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;


import giis.selema.framework.junit4.LifecycleJunit4Class;
import giis.selema.framework.junit4.LifecycleJunit4Test;
import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SelemaConfig;
import giis.selema.manager.SelemaException;
import giis.selema.portable.selenium.DriverUtil;
import giis.selema.manager.SeleManager;
import giis.selema.services.IMediaContext;
import giis.selema.services.impl.MediaContext;
import giis.selema.services.impl.WatermarkService;

/**
 * Checks exceptional situations: Some of them do not raise exceptions, but write in the selema log, 
 * others should raise exception to the user.
 * Note that exceptions are tested using try catch to allow automatic conversion to nunit
 */
public class TestExceptions implements IAfterEachCallback {
	//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
	final static Logger log=LoggerFactory.getLogger(TestExceptions.class);
	protected static LifecycleAsserts lfas=new LifecycleAsserts();
	protected String currentName() { return "TestExceptions"; }
	protected static int thisTestCount=0;

	//this sm includes a configuration of the driver (check that driver runs maximized)
	protected static SeleManager sm=new SeleManager(Config4test.getConfig())
			.setManagerDelegate(new Config4test(false))
			.setManageAtClass()
			.setDriverDelegate(new DriverConfigMaximize());
	@ClassRule public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
	@Rule public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm);

	protected static WebDriver saveDriver;
	@Override
	public void runAfterCallback(String testName, boolean success) { 
		new AfterEachCallback(lfas, log, sm).runAfterCallback(testName, success);
	}
	//Dirty: Driver is managed at class level for performance
	//but some tests manipulate the driver, save an restore for each test
	@Before
	public void setUp() {
		saveDriver=sm.driver();
		sm.add((WatermarkService)null); // some tests add this service, this removes it
		lfas.assertAfterSetup(sm, false, thisTestCount==0); //ensures correct setup
		launchPage();
	}
	@After
	public void tearDown() {
		thisTestCount++; //only 0 in first test
		sm.replaceDriver(saveDriver);
	}

	protected void launchPage() {
		DriverUtil.getUrl(sm.driver(), new Config4test().getWebUrl()); //siempre usa la misma pagina
	}

	@Test
	public void testManagerWithoutConfig() {
		RuntimeException e = assertThrows(SelemaException.class, () -> {
			new SeleManager(null);
		});
		assertEquals("SeleManager instance requires an instance of SelemaConfig", e.getMessage());
	}
	
	//uses a different report subdir to do not include wrong named files/folders that cause error when published as artifacts
	@Test
	public void testManagerWrongName() {
		assertThrows(RuntimeException.class, () -> {
			new SeleManager(new SelemaConfig().setReportSubdir("dat/tmp").setName("ab\0"));
		});
	}
	@Test
	public void testManagerWrongReportSubdir() {
		assertThrows(RuntimeException.class, () -> {
			new SeleManager(new SelemaConfig().setReportSubdir("dat/tmp/ab\0"));
		});
	}
	@Test
	public void testManagerWrongProjectRoot() {
		assertThrows(RuntimeException.class, () -> {
			new SeleManager(new SelemaConfig().setProjectRoot("dat/tmp/ab\0"));
		});
	}
	@Test
	public void testScreenshotExceptionByDriver() {
		//first screenshot pass
		sm.screenshot("forced-screenshot");
		lfas.assertLast("[INFO]", "Taking screenshot", "forced-screenshot.png");
		//forces exception by passing a null driver
		IMediaContext context=new MediaContext(sm.getConfig().getReportDir(), sm.getConfig().getQualifier(), 99, 99);
		sm.getScreenshotService().takeScreenshot(null, context, "TestExceptions.testScreenshotInternalException");
		lfas.assertLast("[ERROR]", "Can't take screenshot or write the content to file", "TestExceptions-testScreenshotInternalException.png");
	}
	@Test
	public void testScreenshotExceptionWriting() {
		//forces exception writing by pasing an invalid report dir
		IMediaContext context=new MediaContext(sm.getConfig().getProjectRoot() + "/dat/tmp/ab\0", sm.getConfig().getQualifier(), 99, 99);
		sm.getScreenshotService().takeScreenshot(sm.driver(), context, "TestExceptions.testScreenshotInternalException");
		lfas.assertLast("[ERROR]", "Can't take screenshot or write the content to file", "TestExceptions-testScreenshotInternalException.png");
	}
	@Test
	public void testVisualAssertException() {
		sm.visualAssertEquals("ab cd", "ab cd"); //first assert pass
		assertThrows(AssertionError.class, () -> {
			sm.visualAssertEquals("ab cd", "ab xy cd");
		});
		lfas.assertLast("[WARN]", "Visual Assert differences", "TestExceptions-testVisualAssertException.html");
		
		//with message
		assertThrows(AssertionError.class, () -> {
			sm.visualAssertEquals("ef gh", "ef zt gh", "va message");
		});
		lfas.assertLast("[WARN]", "Visual Assert differences", "TestExceptions-testVisualAssertException.html", "va message");
	}
	@Test
	public void testSoftAssertException() {
		sm.softAssertClear();
		sm.softAssertEquals("ab cd", "ab cd"); //first assert pass
		sm.softAssertEquals("ab cd", "ab xy cd");
		sm.softAssertEquals("ef gh", "ef zt gh", "sva message");
		assertThrows(AssertionError.class, () -> {
			sm.softAssertAll();
		});
		lfas.assertLast(0, "[WARN]", "Soft Visual Assert differences (Failure 2)", "TestExceptions-testSoftAssertException.html", "sva message");
		lfas.assertLast(1, "[WARN]", "Soft Visual Assert differences (Failure 1)", "TestExceptions-testSoftAssertException.html");
	}
	@Test
	public void testWatermarkExceptionNotAttached() {
		assertThrows(RuntimeException.class, () -> {
			sm.watermark();
		});
		lfas.assertLast("[ERROR]", "Watermark service is not attached to this Selenium Manager");
	}
	@Test
	public void testWatermarkExceptionWritingMessage() {
		// Driver will be closed, needs a new one for this test
		sm.replaceDriver(sm.createDriver());
		sm.add(new WatermarkService());
		sm.driver().close();
		// Failure to write because the driver is closed causes error messages about the watermark, but not exception
		sm.watermarkText("thisShouldNotFail");
		lfas.assertLast("[WARN]", "Can't write onFailure watermark thisShouldNotFail");
	}
	@Test
	public void testWatermarkExceptionWritingOnFailureMessage() {
		// Driver will be closed, needs a new one for this test
		sm.replaceDriver(sm.createDriver());
		sm.add(new WatermarkService());
		sm.watermarkText("this works");
		// Failure to write because the driver is closed causes error messages about the watermark
		sm.driver().close();
		sm.onFailure("thisClass", "thisTest");
		lfas.assertLast("[ERROR]", "Can't write onFailure watermark thisTest");
	}

}
