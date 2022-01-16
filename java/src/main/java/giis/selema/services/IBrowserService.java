package giis.selema.services;

import java.util.Map;

public interface IBrowserService {

	/** 
	 * Activates the video recording, provided that the service is configured for video recording
	 */ 
	IBrowserService setVideo();

	/** 
	 * Activates the VNC capabilities to be able to watch the test execution in real time
	 */ 
	IBrowserService setVnc();

	/**
	 * Gets the video recorder service associated with this browser service
	 */
	IVideoService getVideoRecorder();

	/**
	 * Gets the capabilities that the WebDriver must configure to integrate with this service
	 */
	Map<String, Object> getSeleniumOptions(String sessionName);

}