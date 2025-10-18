package giis.selema.services.impl;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.portable.util.FileUtil;
import giis.selema.portable.selenium.RestClient;
import giis.selema.services.IVideoController;

/**
 * Video controller to use when the recorder container and the tests run in different or the same VM and start/stop is
 * managed by a separated server (video-controller).
 * 
 * The video-controller server to start the recorder (post), stop and download the video (get) and delete the recorded
 * video to get ready for next session.
 */
public class VideoControllerRemote implements IVideoController {
	final Logger log = LoggerFactory.getLogger(this.getClass());

	private static final String PATH = "/recording";
	private String endpoint;
	private String targetFolder;

	/**
	 * Creates a new instance
	 * 
	 * @param label an identifier of the pair of selenium node and video recorder, that will be passed as a path parameter
	 * @param controllerUrl the endpoint of the video-controller
	 * @param targetFolder where the recorded videos will be stored after recording
	 */
	public VideoControllerRemote(String label, String controllerUrl, String targetFolder) {
		this.endpoint = controllerUrl + PATH + "/" + label;
		this.targetFolder = targetFolder;
	}

	@Override
	public void start() {
		RestClient.restCall("POST", this.endpoint);
	}

	@Override
	public void stop(String videoName) {
		String targetFile = FileUtil.getPath(this.targetFolder, videoName);
		
		// Call the api to download and then delete the video file
		RestClient.restDownload(this.endpoint, targetFile);
		RestClient.restCall("DELETE", this.endpoint);
	}

}
