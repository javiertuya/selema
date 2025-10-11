package test4giis.selema.portable;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;

import org.hamcrest.CoreMatchers;

/**
 * Asserts for compatibility in translation from JUnit4 to NUnit3  (asserts with messages)
 */
public class Asserts {
	private Asserts() {
		throw new IllegalStateException("Utility class");
	}
	
	public static void assertAreEqual(String expected, String actual, String message) {
		assertEquals(message, expected, actual);		
	}

	public static void assertIsTrue(boolean condition, String message) {
		assertTrue(message, condition);		
	}
	
	public static void assertContains(String expectedSubstring, String actual) {
		assertThat(actual, CoreMatchers.containsString(expectedSubstring));
	}

}
