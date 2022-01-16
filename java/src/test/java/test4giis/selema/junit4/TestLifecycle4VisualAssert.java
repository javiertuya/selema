package test4giis.selema.junit4;

import static org.junit.Assert.fail;

import org.junit.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.framework.junit4.LifecycleJunit4Class;
import giis.selema.framework.junit4.LifecycleJunit4Test;
import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleniumManager;
import test4giis.selema.core.AfterEachCallback;
import test4giis.selema.core.Config4test;
import test4giis.selema.core.LifecycleAsserts;
import test4giis.selema.core.LogReader;

/**
 * Prueba de un visual assert dentro del ciclo de vida (para comparacion manual)
 */
public class TestLifecycle4VisualAssert implements IAfterEachCallback {
	//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
	final static Logger log=LoggerFactory.getLogger(TestLifecycle4.class);
	protected static LifecycleAsserts lfas=new LifecycleAsserts();
	
	protected static SeleniumManager sm=new SeleniumManager(Config4test.getConfig()).setManagerDelegate(new Config4test());
	@ClassRule public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
	@Rule public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm, new AfterEachCallback(lfas, log, sm));

	@Override
	public void runAfterCallback(String testName, boolean success) { 
		new AfterEachCallback(lfas, log, sm).runAfterCallback(testName, success);
	}

	@Test
	public void testVisualAssertEquals() { 
		String actual="uno dos tres\nabc def ghi";
		String expected="uno tres\nabc XXXX def ghi";
		//Captura la excepcion del assert para comprobar el mensaje
		try {
			sm.visualAssertEquals(expected, actual, "ADDITIONAL MESSAGE");
			fail("this assert should fail");
		} catch (AssertionError e) {
			LogReader reader=lfas.getLogReader();
			reader.assertBegin();
			reader.assertContains("Visual Assert differences", "testVisualAssertEquals");
			reader.assertEnd();
		}
	}

}
