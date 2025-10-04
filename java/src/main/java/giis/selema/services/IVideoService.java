package giis.selema.services;

import java.util.Map;

import org.openqa.selenium.WebDriver;

/**
 * Support for video recording for a browser service.
 * The instance of IBrowserService should create an instance of IVideoService for each Selenium Session
 */
public interface IVideoService {

	/**
	 * Configures this service, called on attaching the service to a SeleManager
	 */
	IVideoService configure(ISelemaLogger thisLog);
	
	void beforeCreateDriver();

	void afterCreateDriver(WebDriver driver);

	String onTestFailure(IMediaContext context, String testName);

	/**
	 * Gets the capabilities that the WebDriver must configure to integrate with this service
	 */
	public Map<String, Object> getSeleniumOptions(IMediaContext context, String testName);

	void beforeQuitDriver(IMediaContext context, String testName);

}