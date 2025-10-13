package giis.selema.services.impl;

import java.util.HashMap;
import java.util.Map;

import giis.selema.services.IBrowserService;
import giis.selema.services.IMediaContext;
import giis.selema.services.IVideoService;

/**
 * Base class for all remote browser services
 */
public abstract class AbstractBrowserService implements IBrowserService {
	protected boolean recordVideo = false; // si true habilita el video recording
	protected boolean enableVnc = false; // si true habilita VNC par ver las sesiones desde selenoid-ui
	protected Map<String, Object> specialCapabilities = new HashMap<String, Object>(); // NOSONAR net compatibility

	/** 
	 * Activates the video recording, provided that the browser server is configured for video recording
	 */ 
	@Override
	public IBrowserService setVideo() {
		this.recordVideo = true;
		return this;
	}
	/** 
	 * Activates the VNC capabilities to be able to watch the test execution in real time (e.g. using selenoid-ui)
	 */ 
	@Override
	public IBrowserService setVnc() {
		enableVnc = true;
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
	 */
	@Override
	public void addBrowserServiceOptions(Map<String, Object> allOptions, IVideoService videoRecorder,
			IMediaContext mediaVideoContext, String driverScope) {
		// Unlike selenoid, by default, browser specific options in grid are not grouped
		allOptions.putAll(this.getSeleniumOptions(driverScope));
		if (videoRecorder != null)
			allOptions.putAll(videoRecorder.getSeleniumOptions(mediaVideoContext, driverScope));
	}

	/**
	 * Gets a new instance of the video recorder service associated with this browser service
	 */
	@Override
	public abstract IVideoService getVideoRecorder();

	/**
	 * Gets the capabilities that the WebDriver must configure to integrate with this service
	 */
	@Override
	public Map<String, Object> getSeleniumOptions(String sessionName) {
		Map<String, Object> opts = new HashMap<String, Object>(); // NOSONAR net compatibility
		// By default, no additional options are added
		opts.putAll(specialCapabilities);
		return opts;
	}

}
