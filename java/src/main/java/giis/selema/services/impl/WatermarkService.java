package giis.selema.services.impl;

import org.openqa.selenium.WebDriver;

import giis.selema.portable.JavaCs;
import giis.selema.portable.selenium.SeleniumActions;
import giis.selema.services.IWatermarkService;

/**
 * Management of watermarks that are placed in the web page under test
 */
public class WatermarkService implements IWatermarkService {
	private String elementId;
	private int delay;
	private String backgroundColor;
	public WatermarkService() {
		setElementId("selema-watermark");
		setDelayOnFailure(1);
	}
	/**
	 * Sets the name of de web element id that will store the watermarks
	 */
	@Override
	public IWatermarkService setElementId(String waternarkElementId) {
		this.elementId=waternarkElementId;
		return this;
	}
	/**
	 * After a test failure, waits for the specified time in seconds to give more time to watch the state of the browser (interactively or in a video)
	 */
	@Override
	public IWatermarkService setDelayOnFailure(int delayOnFailure) {
		this.delay=delayOnFailure;
		return this;
	}
	/**
	 * Sets a background color to better differentiate the watermark from the web content (by default watermark has no background)
	 */
	@Override
	public IWatermarkService setBackground(String color) {
		this.backgroundColor=color;
		return this;
	}
	
	/**
	 * Inserts a normal watermark (green)
	 */
	@Override
	public void write(WebDriver driver, String value) {
		writeToBrowser(driver,value,"","darkgreen");
	}
	/**
	 * Inserts a failure watermark (red)
	 */
	@Override
	public void fail(WebDriver driver, String value) {
		writeToBrowser(driver, value, "FAIL ", "red");
		for (int i=0; i<delay; i++) {
			JavaCs.sleep(1000);
			value += " ."; //NOSONAR
			writeToBrowser(driver, value, "FAIL ", "red");
		}
	}
	
	// Creates the element to hold the watermar (if does not exists) and writes the text
	private void writeToBrowser(WebDriver driver, String value, String prefix, String color) {
		String js="var elem=document.getElementById('" + this.elementId + "'); "
				+ "if (elem==null) { "
				+ "  var spn = document.createElement('span'); "
				+ "  spn.setAttribute('id','" + this.elementId + "'); "
				+ "  document.body.appendChild(spn); "
				+ "  elem=spn; "
				+ "} "
				+ "if (elem!=null) { "
				+ "  elem.style.position='absolute'; "
				+ "  elem.style.left='" + "1px" + "'; "
				+ "  elem.style.top='" + "1px" + "'; "
				+ "  elem.style.fontSize='" + "small" + "'; "
				+ (this.backgroundColor==null || "".equals(backgroundColor) ? "" 
						: "  elem.style.background='" + this.backgroundColor + "'; ")
				+ "  elem.style.color='" + color + "'; "
				+ "  elem.textContent='" + prefix+value + "'; "
				+ "}";
		SeleniumActions.executeScript(driver, js);
	}

}
