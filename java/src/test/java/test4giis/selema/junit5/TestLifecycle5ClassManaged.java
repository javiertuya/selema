package test4giis.selema.junit5;

import static org.junit.Assert.assertEquals;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.openqa.selenium.By;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.framework.junit5.LifecycleJunit5;
import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleManager;
import giis.selema.portable.Parameters;
import test4giis.selema.core.AfterEachCallback;
import test4giis.selema.core.LifecycleAsserts;
import test4giis.selema.core.Config4test;

@ExtendWith(LifecycleJunit5.class)
public class TestLifecycle5ClassManaged implements IAfterEachCallback {
	final static Logger log=LoggerFactory.getLogger(TestLifecycle5ClassManaged.class);
	protected static LifecycleAsserts lfas=new LifecycleAsserts();
	protected String currentName() { return "TestLifecycle5ClassManaged"; }
	protected static int thisTestCount=0;

	protected static SeleManager sm=new SeleManager(Config4test.getConfig()).setManagerDelegate(new Config4test()).setManageAtClass();

	@Override
	public void runAfterCallback(String testName, boolean success) { 
		new AfterEachCallback(lfas, log, sm).runAfterCallback(testName, success);
	}

	protected void launchPage() {
		sm.driver().get(new Config4test().getWebUrl()); //siempre usa la misma pagina
		sm.watermark();
	}

	@Test
	public void testFailMethod() {
		assertEquals(currentName()+".testFailMethod", sm.currentTestName());
		lfas.assertAfterSetup(sm, false, thisTestCount==0);
		thisTestCount++;
		launchPage();
		sm.onFailure(currentName(), sm.currentTestName());
		lfas.assertAfterFail(sm);
	}
	@Test
	public void testPassMethod() {
		assertEquals(currentName()+".testPassMethod",sm.currentTestName());
		lfas.assertAfterSetup(sm, false, thisTestCount==0);
		thisTestCount++;
		launchPage();
		sm.getLogger().info("INSIDE TEST BODY");
		assertEquals("result",sm.driver().findElement(By.id("spanAlert")).getText());
		lfas.assertAfterPass();
	}

}
