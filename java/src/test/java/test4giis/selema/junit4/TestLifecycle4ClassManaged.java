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
import test4giis.selema.core.AfterEachCallback;
import test4giis.selema.core.LifecycleAsserts;
import test4giis.selema.core.Config4test;
import test4giis.selema.core.DriverConfigMaximize;

public class TestLifecycle4ClassManaged implements IAfterEachCallback {
	//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
	final static Logger log=LoggerFactory.getLogger(TestLifecycle4ClassManaged.class);
	protected static LifecycleAsserts lfas=new LifecycleAsserts();
	protected String currentName() { return "TestLifecycle4ClassManaged"; }
	protected static int thisTestCount=0;

	//this sm includes a configuration of the driver (check that driver runs maximized)
	protected static SeleniumManager sm=new SeleniumManager(Config4test.getConfig()).setManagerDelegate(new Config4test()).setManageAtClass().setDriverDelegate(new DriverConfigMaximize());
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

	@Test
	public void testFailMethod() {
		lfas.assertNow(currentName()+".testFailMethod", sm.currentTestName());
		lfas.assertAfterSetup(sm, false, thisTestCount==0);
		thisTestCount++;
		launchPage();
		sm.onFailure(currentName(), sm.currentTestName());
		lfas.assertAfterFail(sm);
	}
	@Test
	public void testPassMethod() {
		lfas.assertNow(currentName()+".testPassMethod",sm.currentTestName());
		lfas.assertAfterSetup(sm, false, thisTestCount==0);
		thisTestCount++;
		launchPage();
		sm.getLogger().info("INSIDE TEST BODY");
		assertEquals("result",sm.driver().findElement(By.id("spanAlert")).getText());
		lfas.assertAfterPass();
	}

}
