package giis.selema.services;

/**
 * Management of SoftVisualAssert comparisons between long strings, managing the html diff files and links from the log
 */
public interface ISoftAssertService {

	/**
	 * Configures this service, called on attaching the service to a SeleManager
	 */
	public ISoftAssertService configure(ISelemaLogger logger, boolean local, String projectRoot, String reportSubdir);
	
	/**
	 * General soft assertion, intended to be used from a proxy in the SeleManager class
	 */
	void assertEquals(String expected, String actual, String message, IMediaContext context, String testName);

	/**
	 * Throws and exception if at least one assertion failed including all assertion messages
	 */
	void assertAll();

	/**
	 * Resets the current failure messages that are stored
	 */
	void assertClear();
	
}