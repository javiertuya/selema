package giis.selema.services.browser;

import org.openqa.selenium.WebDriver;

import giis.selema.services.IMediaContext;
import giis.selema.services.IVideoController;

/**
 * Manages the start/stop of the video recording container when the selenium session is open/closed (driver
 * created/closed).
 */
public class RemoteVideoService extends AbstractVideoService {

	// For preloaded services, an additional instance for controller the video recording is required
	private IVideoController videoController;

	public RemoteVideoService(IVideoController videoController) {
		this.videoController = videoController;
	}

	@Override
	public void afterCreateDriver(WebDriver driver) {
		super.afterCreateDriver(driver);
		videoControllerStart();
	}

	@Override
	public void beforeQuitDriver(IMediaContext context, String testName) {
		String videoFileName = getVideoFileNameWithRelativePath(context, testName);
		videoControllerStop(videoFileName);
		super.beforeQuitDriver(context, testName);
	}

	private void videoControllerStart() {
		try {
			videoController.start();
		} catch (Exception e) { //
			logSelema.error("Video controller start failure - " + e.getMessage());
		}
	}

	private void videoControllerStop(String videoFileName) {
		try {
			videoController.stop(videoFileName);
		} catch (Exception e) { //
			logSelema.error("Video controller stop failure - " + e.getMessage());
		}
	}

}
