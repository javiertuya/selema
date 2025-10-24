package giis.selema.services;

import java.util.Map;

/**
 * This service must be aded to the selema manager to get additional functionalities 
 * of video recording or VNC to watch the test execution
 */
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
	 * Adds a special capability other than the predefined video and vnc
	 * that are specific to the remote browser server
	 */ 
	IBrowserService setBrowserCapability(String key, Object value);

	/**
	 * Adds the browser service specific capabilities to 'allOptions' map.
	 */
	void addBrowserServiceOptions(Map<String, Object> allOptions, IVideoService videoRecorder,
			IMediaContext mediaVideoContext, String driverScope);
	
	/**
	 * Gets a new instance of the video recorder service associated with this browser service to manage a selenium session
	 */
	IVideoService getNewVideoRecorder();

}