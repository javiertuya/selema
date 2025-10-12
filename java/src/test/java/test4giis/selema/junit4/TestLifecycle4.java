package test4giis.selema.junit4;

import static org.junit.Assert.assertEquals;

import org.junit.ClassRule;
import org.junit.Rule;
import org.junit.Test;
import org.openqa.selenium.By;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.framework.junit4.LifecycleJunit4Class;
import giis.selema.framework.junit4.LifecycleJunit4Test;
import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleManager;
import giis.selema.portable.selenium.DriverUtil;
import test4giis.selema.core.Config4test;
import test4giis.selema.core.AfterEachCallback;
import test4giis.selema.core.LifecycleAsserts;

/**
 * Comprueba las acciones de test que fallan y pasan, junto con los logs generados que confirman las acciones realizadas
 * cuando se tiene una sesion de driver por cada uno de los test.
 * Comprobar tambien de forma manual:
 * - las imagenes y videos, incluyendo los enlaces incluidos en el log
 * - lo anterior desde eclipse y desde el log de Jenkins
 */
public class TestLifecycle4 implements IAfterEachCallback {
	//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
	final static Logger log=LoggerFactory.getLogger(TestLifecycle4.class);
	protected static LifecycleAsserts lfas=new LifecycleAsserts();
	protected String currentName() { return "TestLifecycle4"; }
	
	protected static SeleManager sm=new SeleManager(Config4test.getConfig()).setManagerDelegate(new Config4test()).setManageAtTest();
	@ClassRule public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
	@Rule public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm, new AfterEachCallback(lfas, log, sm));

	@Override
	public void runAfterCallback(String testName, boolean success) { 
		new AfterEachCallback(lfas, log, sm).runAfterCallback(testName, success);
	}
	
	protected void launchPage() {
		DriverUtil.getUrl(sm.driver(), new Config4test().getWebUrl()); //siempre usa la misma pagina
		sm.watermark();
	}

	@Test
	public void testMethod1() {
		launchPage();
		sm.getLogger().info("INSIDE TEST BODY");
	}
	@Test
	public void testMethod2() {
		launchPage();
		sm.getLogger().info("INSIDE TEST BODY");
	}
	@Test
	public void testMethod3() {
		launchPage();
		sm.getLogger().info("INSIDE TEST BODY");
	}
	@Test
	public void testMethod4() {
		launchPage();
		sm.getLogger().info("INSIDE TEST BODY");
	}
	@Test
	public void testMethod5() {
		launchPage();
		sm.getLogger().info("INSIDE TEST BODY");
	}
	
}
