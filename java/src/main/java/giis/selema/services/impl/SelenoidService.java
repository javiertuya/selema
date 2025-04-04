package giis.selema.services.impl;

import java.util.HashMap;
import java.util.Map;

import giis.selema.services.IBrowserService;
import giis.selema.services.IVideoService;

public class SelenoidService implements IBrowserService {
	private boolean recordVideo=false; //si true habilita el video recording
	private boolean enableVnc=false; //si true habilita VNC par ver las sesiones desde selenoid-ui
	private Map<String, Object> specialCapabilities = new HashMap<String, Object>(); // NOSONAR net compatibility

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
