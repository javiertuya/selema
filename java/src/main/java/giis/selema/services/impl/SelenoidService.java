package giis.selema.services.impl;

import java.util.HashMap;
import java.util.Map;

import giis.selema.services.IBrowserService;
import giis.selema.services.IMediaContext;
import giis.selema.services.IVideoService;

public class SelenoidService implements IBrowserService {
	protected boolean recordVideo=false; //si true habilita el video recording
	protected boolean enableVnc=false; //si true habilita VNC par ver las sesiones desde selenoid-ui
	protected Map<String, Object> specialCapabilities = new HashMap<String, Object>(); // NOSONAR net compatibility

	/** 
	 * Activates the video recording, provided that the Selenoid server is configured for video recording
	 */ 
	@Override
	public IBrowserService setVideo() {
		this.recordVideo=true;
		return this;
	}
	/** 
	 * ctivates the VNC capabilities to be able to watch the test execution in real time (e.g. using selenoid-ui)
	 */ 
	@Override
	public IBrowserService setVnc() {
		enableVnc=true;
		return this;
	}
	/** 
	 * Adds a special capability other than the predefined video and vnc
	 */ 
	@Override
	public IBrowserService setCapability(String key, Object value) {
		specialCapabilities.put(key, value);
		return this;
	}
	
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
	 * Gets the video recorder service associated with this browser service
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
