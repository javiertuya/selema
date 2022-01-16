package giis.selema.services;

import org.openqa.selenium.WebDriver;
/**
 * Management of watermarks that are placed in the web page under test
 */
public interface IWatermarkService {

	/**
	 * Sets the name of de web element id that will store the watermarks
	 */
	IWatermarkService setElementId(String waternarkElementId);

	/**
	 * After a test failure, waits for the specified time in seconds to give more time to watch the state of the browser (interactively or in a video)
	 */
	IWatermarkService setDelayOnFailure(int delayOnFailure);

	/**
	 * Inserts a normal watermark (green)
	 */
	void write(WebDriver driver, String value);

	/**
	 * Inserts a failure watermark (red)
	 */
	void fail(WebDriver driver, String value);

	/**
	 * Sets a background color to better differentiate the watermark from the web content (by default watermark has no background)
	 */
	IWatermarkService setBackground(String color);

}