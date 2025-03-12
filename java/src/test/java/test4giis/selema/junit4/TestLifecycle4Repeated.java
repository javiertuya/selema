package test4giis.selema.junit4;

import static org.junit.Assert.assertTrue;
import static org.junit.Assert.fail;

import org.junit.Before;
import org.junit.ClassRule;
import org.junit.Rule;
import org.junit.Test;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.framework.junit4.LifecycleJunit4Class;
import giis.selema.framework.junit4.LifecycleJunit4Test;
import giis.selema.framework.junit4.RepeatedIfExceptionsTest;
import giis.selema.framework.junit4.RepeatedTestRule;
import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleManager;
import giis.selema.portable.selenium.DriverUtil;
import giis.selema.services.impl.WatermarkService;
import test4giis.selema.core.AfterEachCallback;
import test4giis.selema.core.LifecycleAsserts;
import test4giis.selema.core.Config4test;

public class TestLifecycle4Repeated implements IAfterEachCallback {
	final static Logger log=LoggerFactory.getLogger(TestLifecycle4Repeated.class);
	protected static LifecycleAsserts lfas=new LifecycleAsserts();

	protected static SeleManager sm=new SeleManager(Config4test.getConfig())
			.setManagerDelegate(new Config4test())
			.add(new WatermarkService())
			.setMaximize(true);
	@ClassRule public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
	@Rule public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm, new AfterEachCallback(lfas, log, sm));
	@Rule public RepeatedTestRule repeatRule = new RepeatedTestRule(sm, new AfterEachCallback(lfas, log, sm));

	@Override
	public void runAfterCallback(String testName, boolean success) { 
		new AfterEachCallback(lfas, log, sm).runAfterCallback(testName, success);
	}
	
	@Before
	public void setUp() {
		DriverUtil.getUrl(sm.driver(), new Config4test().getWebUrl());
		sm.watermark();
	}

	private static int repetitions=0;
 
	@RepeatedIfExceptionsTest(repeats = 3)
	@Test
	public void testRepeated() {
		lfas.assertAfterSetup(sm, true);
		repetitions++;
		if (repetitions<3) //falla salvo la ultima repeticion
			fail("simulated failure");
		else if (new Config4test().getManualCheckEnabled()) //siempre falla para comprobacion manual
			fail("Manual Check: salida estandar del test muestra nombres de test y log de grabacion video y screenshot");
		assertTrue(true);
	}

}
