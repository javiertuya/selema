package giis.selema.services;

import org.openqa.selenium.WebDriver;

/**
 * Additional support to measure javascript coverage
 */
public interface IJsCoverageService {

	/**
	 * Configures this service, called on attaching the service to a SeleniumManager
	 */
	IJsCoverageService configure(ISelemaLogger thisLog, String reportDir);
	
	/**
	 * To be executed after driver creation
	 */
	void afterCreateDriver(WebDriver driver);

	/**
	 * To be executed before driver ends
	 */
	void beforeQuitDriver(WebDriver driver);

}