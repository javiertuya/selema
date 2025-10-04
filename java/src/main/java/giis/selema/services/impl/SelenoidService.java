package giis.selema.services.impl;

import java.util.HashMap;
import java.util.Map;

import giis.selema.services.IMediaContext;
import giis.selema.services.IVideoService;

public class SelenoidService extends AbstractBrowserService {
	
	/**
	 * Adds the browser service specific capabilities to 'allOptions' map.
	 * In selenoid the capabilities are enclosed under a "selenoid:options" key
	 */
	@Override
	public void addBrowserServiceOptions(Map<String, Object> allOptions, IVideoService videoRecorder,
			IMediaContext mediaVideoContext, String driverScope) {
		//Although browser service and video recorder are handled independently, in the case of Selenoid:
		//-using Selenium 4.1.0 on .NET, options are not passed to the driver
		//-it is required to pass all selenoid related options as WebDriver protocol extension as a pair "selenoid:options", <map with all options>
		Map<String, Object> selenoidOptions = new HashMap<String, Object>(); // NOSONAR net compatibility
		selenoidOptions.putAll(this.getSeleniumOptions(driverScope));
		if (videoRecorder != null)
			selenoidOptions.putAll(videoRecorder.getSeleniumOptions(mediaVideoContext, driverScope));
		allOptions.put("selenoid:options", selenoidOptions);
	}

	/**
	 * Gets a new instance of the video recorder service associated with this browser service
	 */
	@Override
	public IVideoService getVideoRecorder() {
		return recordVideo ? new SelenoidVideoService() : null;
	}
	
	/**
	 * Gets the capabilities that the WebDriver must configure to integrate with this service
	 */
	@Override
	public Map<String, Object> getSeleniumOptions(String sessionName) {
		Map<String, Object> opts = new HashMap<String, Object>(); // NOSONAR net compatibility
		opts.put("name", sessionName);
		opts.put("enableVNC", enableVnc);
		opts.putAll(specialCapabilities);
		return opts;
	}

}
