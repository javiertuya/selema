package giis.samples.selema.junit4;
import static org.junit.Assert.assertEquals;

import org.junit.ClassRule;
import org.junit.Rule;
import org.junit.Test;
import giis.selema.manager.SeleniumManager;
import giis.selema.framework.junit4.LifecycleJunit4Class;
import giis.selema.framework.junit4.LifecycleJunit4Test;
import giis.selema.framework.junit4.RepeatedIfExceptionsTest;
import giis.selema.framework.junit4.RepeatedTestRule;

/**
 * Selenium Test Lifecycle Manager (selema) sample on the JUnit 5 framework, 
 * see usage at https://github.com/javiertuya/selema#readme
 */
public class TestSampleJunit4 {
	private static SeleniumManager sm=new SeleniumManager().setBrowser("chrome");
	//ClassRule is needed when driver is managed at the class level
	@ClassRule public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm); //required only if using a driver per class
	@Rule public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm);
	@Rule public RepeatedTestRule repeatRule = new RepeatedTestRule(sm); //required for retry tests

	@Test
	public void testFailMethod() {
		sm.driver().get("https://en.wikipedia.org/");
		assertEquals("XXXX Wikipedia, the free encyclopedia", sm.driver().getTitle());
	}
	
	//Repeated tests demo, uses a counter to simulate failures
	private static int repetitions=0;
	 
	@RepeatedIfExceptionsTest(repeats = 3)
	@Test
	public void testRepeated() {
		repetitions++;
		String expected="Wikipedia, the free encyclopedia";
		if (repetitions<3) //fails except last repetition
			expected="XXX " + expected;
		sm.driver().get("https://en.wikipedia.org/");
		assertEquals(expected, sm.driver().getTitle());
	}
	
}
