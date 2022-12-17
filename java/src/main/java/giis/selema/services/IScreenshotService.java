package giis.selema.services;

import org.openqa.selenium.WebDriver;

/**
 * Screenshot management
 */
public interface IScreenshotService {

	/**
	 * Configures this service, called on attaching the service to a SeleManager
	 */
	public IScreenshotService configure(ISelemaLogger thisLog);
	
	/**
	 * Takes an picture of the current state of the browser and saves it to the reports folder
	 */
	String takeScreenshot(WebDriver driver, IMediaContext context, String testName);

}