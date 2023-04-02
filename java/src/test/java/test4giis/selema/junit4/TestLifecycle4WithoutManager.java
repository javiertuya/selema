package test4giis.selema.junit4;

import static org.junit.Assert.assertEquals;

import org.junit.AfterClass;
import org.junit.BeforeClass;
import org.junit.ClassRule;
import org.junit.Rule;
import org.junit.Test;

import giis.selema.framework.junit4.LifecycleJunit4Class;
import giis.selema.framework.junit4.LifecycleJunit4Test;
import giis.selema.manager.SelemaException;
import test4giis.selema.core.LifecycleAsserts;
import test4giis.selema.core.LogReader;

public class TestLifecycle4WithoutManager {
	protected static LogReader reader=new LifecycleAsserts().getLogReader();
	protected static int logSize;
	
	@ClassRule public static LifecycleJunit4Class cw = new LifecycleJunit4Class(null);
	@Rule public LifecycleJunit4Test tw = new LifecycleJunit4Test(null);

	@BeforeClass
	public static void setUpClass() {
		logSize=reader.getLogSize();
	}
	@Test
	public void testFailedTestNoWriteLogs() {
		//a failed test should not raise exception nor write logs
		try {
			throw new SelemaException("Exception to be catched");
		} catch (SelemaException e) {
			assertEquals(logSize, reader.getLogSize());
		}
	}
	@AfterClass
	public static void afterTearDown() {
		assertEquals(logSize, reader.getLogSize());
	}

}
