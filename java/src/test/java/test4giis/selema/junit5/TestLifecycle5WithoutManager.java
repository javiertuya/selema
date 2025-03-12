package test4giis.selema.junit5;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

import org.junit.jupiter.api.AfterAll;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;

import giis.selema.framework.junit5.LifecycleJunit5;
import giis.selema.manager.SelemaException;
import test4giis.selema.core.LifecycleAsserts;
import test4giis.selema.core.LogReader;

@ExtendWith(LifecycleJunit5.class)
public class TestLifecycle5WithoutManager {
	protected static LogReader lfas=new LifecycleAsserts().getLogReader();
	protected static int logSize;
	
	@BeforeAll
	public static void setUpClass() {
		logSize=lfas.getLogSize();
	}
	@Test
	public void testFailedTestNoWriteLogs() {
		//a failed test should not raise exception nor write logs
		assertThrows(SelemaException.class, () -> {
			throw new SelemaException("Exception to be catched");
		});
		assertEquals(logSize, lfas.getLogSize());
	}
	@AfterAll
	public static void afterTearDown() {
		assertEquals(logSize, lfas.getLogSize());
	}

}
