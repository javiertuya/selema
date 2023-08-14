package test4giis.selema.core;

import static org.junit.Assert.fail;

import org.junit.Ignore;
import org.junit.Test;
import org.openqa.selenium.WebDriver;

import giis.selema.framework.junit4.Asserts;
import giis.selema.manager.DriverVersion;
import giis.selema.manager.SeleManager;

/**
 * Test on strategies to get the driver version. 
 * 
 * This test class is deactivated by default and should be used for manual test only.
 * Test result comparison should be made on the path and driver file downloaded
 */
@Ignore
public class TestDriverVersion {

	// Get a new SeleManager instance configured for a given version startegy
	private SeleManager newSeleManager(String versionStrategy) {
		SeleManager sm = new SeleManager();
		if (new Config4test().useHeadlessDriver())
			sm.setArguments(TestDriver.chromeHeadlesArgument);
		
		sm.setDriverVersion(versionStrategy);
		return sm;
	}
	@Test
	public void testLocalDriverMatch() {
		SeleManager sm = newSeleManager(DriverVersion.MATCH_BROWSER);
		WebDriver driver = sm.createDriver("ThisClass", "ThisTest");
		driver.close();
	}

	@Test
	public void testLocalDriverLatest() {
		SeleManager sm = newSeleManager(DriverVersion.LATEST_AVAILABLE);
		WebDriver driver = sm.createDriver("ThisClass", "ThisTest");
		driver.close();
	}

	// If some driver has already been downloaded and configured in the path, 
	// SeleniumManager will locate it.
	// If before execution, an old driver has been loaded by next test,
	// and configured in the path, the test will fail
	@Test
	public void testLocalDriverSelenium() {
		SeleManager sm = newSeleManager(DriverVersion.SELENIUM_MANAGER);
		WebDriver driver = sm.createDriver("ThisClass", "ThisTest");
		driver.close();
	}

	// Forces an old driver incompatible with current browsers that will raise exception
	@Test
	public void testLocalDriverGivenVersion() {
		SeleManager sm = newSeleManager("99.0.4844.51");
		try {
			sm.createDriver("ThisClass", "ThisTest");
			fail("Should fail");
		} catch (RuntimeException e) {
			Asserts.assertIsTrue( e.getMessage().contains("This version of ChromeDriver only supports Chrome version 99"),
					"Not contained in: " + e.getMessage());
		}
	}

}
