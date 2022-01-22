package test4giis.selema.junit5;

import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertTrue;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.fail;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.openqa.selenium.WebDriver;

import giis.selema.framework.junit5.LifecycleJunit5;
import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleniumManager;
import test4giis.selema.core.Config4test;
import test4giis.selema.core.AfterEachCallback;
import test4giis.selema.core.LifecycleAsserts;

@ExtendWith(LifecycleJunit5.class)
public class TestLifecycle5Unmanaged implements IAfterEachCallback {
	final static Logger log=LoggerFactory.getLogger(TestLifecycle5Unmanaged.class);
	protected static LifecycleAsserts lfas=new LifecycleAsserts();
	protected String currentName() { return "TestLifecycle5Unmanaged"; }
	
	protected static SeleniumManager sm=new SeleniumManager(Config4test.getConfig()).setManagerDelegate(new Config4test()).setManageNone();

	@Override
	public void runAfterCallback(String testName, boolean success) { 
		new AfterEachCallback(lfas, log, sm).runAfterCallback(testName, success);
	}

	@Test
	public void testNoDriver() {
		assertEquals(currentName()+".testNoDriver", sm.currentTestName());
		lfas.assertAfterSetup(sm, false);
		//should not have an active driver, accesing to it throws exception
		assertFalse(sm.hasDriver());
		try {
			sm.driver().get(new Config4test().getWebUrl()); //siempre usa la misma pagina
			fail("should fail");
		} catch (Throwable e) {
			assertEquals("The Selenium Manager does not have any active WebDriver", e.getMessage());
		}
		lfas.assertLast("[ERROR]", "The Selenium Manager does not have any active WebDriver");
	}
	@Test
	public void testWithDriver() {
		assertEquals(currentName()+".testWithDriver", sm.currentTestName());
		//even if it is unmanaged, I can create a driver that is bound to the manager
		WebDriver driver=sm.createDriver();
		assertTrue(sm.hasDriver());
		lfas.assertAfterSetup(sm, true);
		sm.getLogger().info("INSIDE TEST BODY");
		driver.get(new Config4test().getWebUrl());
		lfas.assertAfterPass();
		sm.quitDriver(driver);
		//despues de cerrar el driver se guardan los videos, estas acciones se deben comprobar antes del teardown para driver remoto
		if (!"".equals(sm.getDriverUrl())) {
			lfas.getLogReader().assertBegin();
			lfas.getLogReader().assertContains("Saving video", currentName() + "-testWithDriver.mp4");
			lfas.getLogReader().assertContains("Remote session ending");
		}
	}

}
