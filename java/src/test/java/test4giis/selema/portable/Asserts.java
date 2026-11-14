package test4giis.selema.portable;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;

import java.io.DataInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.nio.charset.StandardCharsets;

import org.hamcrest.CoreMatchers;

/**
 * Asserts for compatibility in translation from JUnit4 to NUnit3  (asserts with messages)
 */
public class Asserts {
	// Minimum size of a valid recorded video, the shortest videos recorded in CI take several tens of KB
	private static final long MIN_VIDEO_SIZE = 10000;

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

	/**
	 * Checks that a recorded video file exists, is not empty or truncated and has the mp4 signature
	 * ('ftyp' box at bytes 4-7)
	 */
	public static void assertVideoFile(String fileName) {
		File file = new File(fileName);
		assertTrue("Video file should exist: " + fileName, file.exists());
		assertTrue("Video file should have at least " + MIN_VIDEO_SIZE + " bytes, but has " + file.length() + ": " + fileName,
				file.length() >= MIN_VIDEO_SIZE);
		byte[] header = new byte[8];
		try (DataInputStream in = new DataInputStream(new FileInputStream(file))) {
			in.readFully(header);
		} catch (IOException e) {
			throw new AssertionError("Can't read the header of video file: " + fileName, e);
		}
		assertEquals("Video file should have the mp4 signature: " + fileName, "ftyp",
				new String(header, 4, 4, StandardCharsets.US_ASCII));
	}

}
