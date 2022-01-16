package giis.selema.framework.junit4;

import static org.junit.Assert.assertEquals;

/**
 * Asserts for compatibility in translation from JUnit4 to NUnit3
 */
public class Asserts {
	private Asserts() {
		throw new IllegalStateException("Utility class");
	}
	
	public static void assertAreEqual(String expected, String actual, String message) {
		assertEquals(message, expected, actual);		
	}

}
