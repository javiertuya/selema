package giis.selema.services;

/**
 * Management of VisualAssert comparisons between long strings, managing the html diff files and links from the log
 */
public interface IVisualAssertService {

	/**
	 * Configures this service, called on attaching the service to a SeleManager
	 */
	public IVisualAssertService configure(ISelemaLogger logger, boolean local, String projectRoot, String reportSubdir);
	
	/**
	 * General assertion, intended to be used from a proxy in the SeleManager class
	 */
	void assertEquals(String expected, String actual, String message, IMediaContext context, String testName);

}