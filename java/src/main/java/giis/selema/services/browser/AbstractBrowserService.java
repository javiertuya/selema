package giis.selema.services.browser;

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
	public IBrowserService setBrowserCapability(String key, Object value) {
		specialCapabilities.put(key, value);
		return this;
	}
	
	/**
	 * Adds the browser service specific capabilities to 'allOptions' map.
	 */
	@Override
	public void addBrowserServiceOptions(Map<String, Object> allOptions, IVideoService videoRecorder,
			IMediaContext mediaVideoContext, String driverScope) {
		allOptions.putAll(this.getSeleniumOptions(driverScope));
		if (videoRecorder != null)
			allOptions.putAll(videoRecorder.getSeleniumOptions(mediaVideoContext, driverScope));
	}

	/**
	 * Gets a new instance of the video recorder service associated with this browser service to manage a selenium session
	 */
	@Override public abstract IVideoService getNewVideoRecorder(); // NOSONAR java redundant, but needed for C# conversion

	/**
	 * Gets the capabilities that the WebDriver must configure to integrate with this service
	 */
	protected Map<String, Object> getSeleniumOptions(String sessionName) { // NOSONAR not all methods will require sessionName
		Map<String, Object> opts = new HashMap<String, Object>(); // NOSONAR net compatibility
		// By default, no additional options are added
		opts.putAll(specialCapabilities);
		return opts;
	}

}
