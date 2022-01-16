package test4giis.selema.junit5;

import org.junit.jupiter.api.BeforeEach;

import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.junit.jupiter.api.Assertions.fail;

import org.junit.jupiter.api.TestInfo;
import org.junit.jupiter.api.extension.ExtendWith;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.framework.junit5.LifecycleJunit5;
import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleniumManager;
import giis.selema.services.impl.WatermarkService;
import io.github.artsok.RepeatedIfExceptionsTest;
import test4giis.selema.core.AfterEachCallback;
import test4giis.selema.core.LifecycleAsserts;
import test4giis.selema.core.Config4test;

/**
 * Caracteristicas especificas de la plataforma JUnit5 (repeticiones y test parametrizados)
 */
@ExtendWith(LifecycleJunit5.class)
public class TestLifecycle5Repeated implements IAfterEachCallback {
	final static Logger log=LoggerFactory.getLogger(TestLifecycle5Repeated.class);
	protected static LifecycleAsserts lfas=new LifecycleAsserts();

	protected static SeleniumManager sm=new SeleniumManager(Config4test.getConfig()).setManagerDelegate(new Config4test()).add(new WatermarkService());

	@Override
	public void runAfterCallback(String testName, boolean success) { 
		new AfterEachCallback(lfas, log, sm).runAfterCallback(testName, success);
	}

	@BeforeEach
	public void setUp(TestInfo testInfo) {
		sm.driver().get(new Config4test().getWebUrl());
		sm.watermark();
	}

	/*
	@ParameterizedTest
	@CsvSource({"Abc,ABC", "abc,ABC"})
	public void testParametrized(String input, String expected) {
		steps.assertAfterSetupx(remoteSession, sm.currentTestName(), true, true);
		if (!testConfig.getManualCheckEnabled())
			assertEquals(expected, input.toUpperCase());
		else
			assertEquals("Manual Check: salida estandar del test muestra nombres de test y log de grabacion video y screenshot",
					expected, input.toLowerCase());
	}
	*/
	
	private static int repetitions=0;
	@RepeatedIfExceptionsTest(repeats=3)
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
