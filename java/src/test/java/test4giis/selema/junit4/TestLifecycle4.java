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
import giis.selema.manager.SeleniumManager;
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
	
	protected static SeleniumManager sm=new SeleniumManager(Config4test.getConfig()).setManagerDelegate(new Config4test()).setManageAtTest();
	@ClassRule public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
	@Rule public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm, new AfterEachCallback(lfas, log, sm));

	@Override
	public void runAfterCallback(String testName, boolean success) { 
		new AfterEachCallback(lfas, log, sm).runAfterCallback(testName, success);
	}
	
	protected void launchPage() {
		sm.driver().get(new Config4test().getWebUrl()); //siempre usa la misma pagina
		sm.watermark();
	}

	/* Uncomment only for debug
	@Before public void setUp() { log.trace("setUp"); }
	@After public void tearDown() { log.trace("tearDown"); }
	@BeforeClass public static void setUpClass() { log.trace("setUpClass"); }
	@AfterClass public static void tearDownClass() { log.trace("tearDownClass"); }
	*/
	
	@Test
	public void testFailMethod() {
		sm.getWatermarkService().setBackground("yellow");
		lfas.assertNow(currentName()+".testFailMethod", sm.currentTestName());
		lfas.assertAfterSetup(sm, true);
		launchPage();
		//simula un fallo para comprobar luego las acciones realizadas a traves del log
		//NOTA aunque se vera fallo en el watermark, a continuacion se ejecutaran las acciones de teardown correspondientes a no fallo y el test pasara
		sm.onFailure(currentName(), sm.currentTestName());
		lfas.assertAfterFail(sm);
	}
	
	@Test
	public void testPassMethod() {
		lfas.assertNow(currentName()+".testPassMethod",sm.currentTestName());
		lfas.assertAfterSetup(sm, true);
		launchPage();
		sm.getLogger().info("INSIDE TEST BODY");
		assertEquals("result",sm.driver().findElement(By.id("spanAlert")).getText());
		lfas.assertAfterPass();
	}
	
}
