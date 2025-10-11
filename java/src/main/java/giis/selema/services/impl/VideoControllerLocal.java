package giis.selema.services.impl;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.portable.util.FileUtil;
import giis.selema.portable.selenium.CommandLine;
import giis.selema.services.IVideoController;

/**
 * Video controller to use when both the recorder container and the tests run in the same VM. Requires the container
 * running the test have access to docker and to the folders when the videos are stored. Any unexpected behaviour raises
 * an exception that must be handled by the caller video service.
 */
public class VideoControllerLocal implements IVideoController {
	final Logger log = LoggerFactory.getLogger(this.getClass());

	private String videoContainer;
	private String sourceFile;
	private String targetFolder;

	public VideoControllerLocal(String videoContainer, String sourceFile, String targetFolder) {
		this.videoContainer = videoContainer;
		this.sourceFile = sourceFile;
		this.targetFolder = targetFolder;
	}

	@Override
	public void start() {
		// First clean the video file (if exists) to prevent the recorder concatenating videos
		CommandLine.fileDelete(sourceFile, false);
		
		// The recorder should be created and stopped in order to start and record video now.
		// If not, stop now
		if (!"exited".equals(ContainerUtil.getContainerStatus(videoContainer))) {
			log.debug("Video recorder " + videoContainer + " is not stopped, restarting");
			ContainerUtil.runDocker("stop", videoContainer);
			ContainerUtil.waitDocker(videoContainer, "Shutdown complete", "", 5);
		}

		log.debug("Starting video recorder: " + videoContainer);
		ContainerUtil.runDocker("start", videoContainer);
		ContainerUtil.waitDocker(videoContainer, "Display", "is open", 5);
	}

	@Override
	public void stop(String videoName) {
		log.debug("Stopping video recorder: " + videoContainer);
		ContainerUtil.runDocker("stop", videoContainer);
		ContainerUtil.waitDocker(videoContainer, "Shutdown complete", "", 5);

		// copy the video file to its destination and then remove, this should not fail
		log.debug("Saving recorded video file to: " + videoName);
		CommandLine.fileCopy(sourceFile, FileUtil.getPath(targetFolder, videoName));
		CommandLine.fileDelete(sourceFile, true);
	}

}
