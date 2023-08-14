package giis.selema.manager;

/**
 * Strategies for selecting the driver version to download
 */
public class DriverVersion {
	public static final String MATCH_BROWSER = "match"; // try match with browser version
	public static final String LATEST_AVAILABLE = "latest"; // use the latest available version
	public static final String SELENIUM_MANAGER = "selenium"; // use the default SeleniumManager
	public static final String DEFAULT = LATEST_AVAILABLE;
}
