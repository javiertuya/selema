package giis.selema.services.impl;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.portable.util.FileUtil;
import giis.portable.util.JavaCs;
import giis.portable.util.Parameters;
import giis.selema.manager.SelemaException;
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

		log.debug("Starting video container: " + videoContainer);
		runDockerWait("start", videoContainer, "Display selenium-chrome:99.0 is open", 5);
	}

	@Override
	public void stop(String videoName) {
		log.debug("Stopping video container: " + videoContainer);
		runDockerWait("stop", videoContainer, "Shutdown complete", 5);

		// copy the video file to its destination and then remove, this should not fail
		log.debug("Saving recorded video file to: " + videoName);
		CommandLine.fileCopy(sourceFile, FileUtil.getPath(targetFolder, videoName));
		CommandLine.fileDelete(sourceFile, true);
	}

	/**
	 * Runs a docker command (start or stop) and waits until the container log contains the expected string.
	 */
	private void runDockerWait(String verb, String container, String expectedLog, int timeoutSeconds) {
		String command = "docker " + verb + " " + container;
		String dockerOut = CommandLine.runCommand(command);
		if (!container.equals(dockerOut.trim()))
			throw new SelemaException(command + " failed. " + dockerOut);

		// poll the docker log until expected string found
		String logOut = "";
		for (int i = 0; i < timeoutSeconds * 10; i++) {
			logOut = CommandLine.runCommand("docker logs --tail 1 " + container);
			if (logOut.contains(expectedLog))
				return;
			JavaCs.sleep(100);
		}
		throw new SelemaException(
				command + " failed. Last docker log does not contain the expected confirmation, but was: " + logOut);
	}

}
