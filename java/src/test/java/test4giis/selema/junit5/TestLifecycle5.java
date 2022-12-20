package test4giis.selema.junit5;

import static org.junit.jupiter.api.Assertions.assertEquals;

import org.junit.jupiter.api.AfterAll;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.openqa.selenium.By;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.framework.junit5.LifecycleJunit5;
import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleManager;
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
@ExtendWith(LifecycleJunit5.class)
public class TestLifecycle5 implements IAfterEachCallback {
	final static Logger log=LoggerFactory.getLogger(TestLifecycle5.class);
	protected static LifecycleAsserts lfas=new LifecycleAsserts();
	protected String currentName() { return "TestLifecycle5"; }
	
	protected static SeleManager sm=new SeleManager(Config4test.getConfig()).setManagerDelegate(new Config4test()).setManageAtTest();

	@Override
	public void runAfterCallback(String testName, boolean success) { 
		new AfterEachCallback(lfas, log, sm).runAfterCallback(testName, success);
	}
	
	protected void launchPage() {
		sm.driver().get(new Config4test().getWebUrl()); //siempre usa la misma pagina
		sm.watermark();
	}

	@BeforeEach public void setUp() { log.trace("setUp"); }
	@AfterEach public void tearDown() { log.trace("tearDown"); }
	@BeforeAll public static void setUpClass() { log.trace("setUpClass"); }
	@AfterAll public static void tearDownClass() { log.trace("tearDownClass"); }
		
	@Test
	public void testFailMethod() {
		assertEquals(currentName()+".testFailMethod", sm.currentTestName());
		lfas.assertAfterSetup(sm, true);
		launchPage();
		//simula un fallo para comprobar luego las acciones realizadas a traves del log
		//NOTA aunque se vera fallo en el watermark, a continuacion se ejecutaran las acciones de teardown correspondientes a no fallo y el test pasara
		sm.onFailure(currentName(), sm.currentTestName());
		lfas.assertAfterFail(sm);
	}
	
	@Test
	public void testPassMethod() {
		assertEquals(currentName()+".testPassMethod",sm.currentTestName());
		lfas.assertAfterSetup(sm, true);
		launchPage();
		sm.getLogger().info("INSIDE TEST BODY");
		assertEquals("result",sm.driver().findElement(By.id("spanAlert")).getText());
		lfas.assertAfterPass();
	}
	
}
