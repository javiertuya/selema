package test4giis.selema.junit4;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.fail;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.junit.ClassRule;
import org.junit.Rule;
import org.junit.Test;
import org.openqa.selenium.WebDriver;

import giis.selema.framework.junit4.LifecycleJunit4Class;
import giis.selema.framework.junit4.LifecycleJunit4Test;
import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleniumManager;
import test4giis.selema.core.Config4test;
import test4giis.selema.core.AfterEachCallback;
import test4giis.selema.core.LifecycleAsserts;

public class TestLifecycle4Unmanaged implements IAfterEachCallback{
	//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
	final static Logger log=LoggerFactory.getLogger(TestLifecycle4Unmanaged.class);
	protected static LifecycleAsserts lfas=new LifecycleAsserts();
	protected String currentName() { return "TestLifecycle4Unmanaged"; }
	
	protected static SeleniumManager sm=new SeleniumManager(Config4test.getConfig()).setManagerDelegate(new Config4test()).setManageNone();
	@ClassRule public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
	@Rule public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm, new AfterEachCallback(lfas, log, sm));

	@Override
	public void runAfterCallback(String testName, boolean success) { 
		new AfterEachCallback(lfas, log, sm).runAfterCallback(testName, success);
	}

	@Test
	public void testNoDriver() {
		lfas.assertNow(currentName()+".testNoDriver", sm.currentTestName());
		lfas.assertAfterSetup(sm, false);
		//no debe haber driver activo
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
		lfas.assertNow(currentName()+".testWithDriver", sm.currentTestName());
		//aunque es unmanaged, uso los metodos de la base para crear y cerrar el driver pero lo mantiene fuera de SeleniumManager
		WebDriver driver=sm.createDriver();
		lfas.assertAfterSetup(sm, true);
		sm.getLogger().info("INSIDE TEST BODY");
		driver.get(new Config4test().getWebUrl()); //siempre usa la misma pagina
		lfas.assertAfterPass();
		sm.quitDriver(driver);
		sm.quitDriver(driver); //ensures can be called multiple times
		//despues de cerrar el driver se guardan los videos, estas acciones se deben comprobar antes del teardown para driver remoto
		if (!"".equals(sm.getDriverUrl())) {
			lfas.getLogReader().assertBegin();
			lfas.getLogReader().assertContains("Saving video", currentName() + "-testWithDriver");
			lfas.getLogReader().assertContains("Remote session ending");
			lfas.getLogReader().assertEnd();
		}
	}

}
