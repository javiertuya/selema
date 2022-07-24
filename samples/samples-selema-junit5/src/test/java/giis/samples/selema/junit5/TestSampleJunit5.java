package giis.samples.selema.junit5;
import static org.junit.jupiter.api.Assertions.assertEquals;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;

import giis.selema.framework.junit5.LifecycleJunit5;
import giis.selema.manager.SeleniumManager;
import io.github.artsok.RepeatedIfExceptionsTest;

/**
 * Selenium Test Lifecycle Manager (selema) sample on the JUnit 5 framework, 
 * see usage at https://github.com/javiertuya/selema#readme
 */
@ExtendWith(LifecycleJunit5.class)
public class TestSampleJunit5 {
	private static SeleniumManager sm=new SeleniumManager().setBrowser("chrome");
	
	@Test
	public void testFailMethod() {
		sm.driver().get("https://javiertuya.github.io/selema/samples/");
		assertEquals("XXXX Selema samples", sm.driver().getTitle());
	}
	
	//Repeated tests demo, uses a counter to simulate failures
	private static int repetitions=0;
	 
	@RepeatedIfExceptionsTest(repeats = 3)
	public void testRepeated() {
		repetitions++;
		String expected="Selema samples";
		if (repetitions<3) //fails except last repetition
			expected="XXX " + expected;
		sm.driver().get("https://javiertuya.github.io/selema/samples/");
		assertEquals(expected, sm.driver().getTitle());
	}
}
