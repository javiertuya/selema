package test4giis.selema.video;

import org.junit.Test;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.portable.util.FileUtil;
import giis.selema.services.IVideoController;
import giis.selema.services.impl.VideoControllerRemote;

/**
 * Partially integrated test of a Video Controller (remote) that calls the video-controller server
 * 
 * Tests inherit from the local video controller tests and share the same mock recorder container. The only difference
 * is where the mock recorded video is stored (under a folder in the video-controller server).
 * 
 * This requires a previous setting to launch the server (with or without a container).
 * See commands at video-controller/setup-controller-local-dev.sh
 */
public class TestVideoControllerRemote extends TestVideoControllerLocal {
	static final Logger log = LoggerFactory.getLogger(TestVideoControllerRemote.class);

	protected static final String CONTROLLER_URL = "http://localhost:4449/selema-video-controller";
	
	@Override
	protected void fileSystemSetup() {
		// In remote, the mapped folder is inside the remote controller app (with or withot container)
		mappedFolder = FileUtil.getPath(ROOT, "../video-controller/app/videos");
		recordedVideo = mappedFolder + "/mock.mp4";
		targetFolder = FileUtil.getPath(ROOT, "..", "video-controller/target/vcmock-target");
	}

	@Override
	protected IVideoController getController() {
		return new VideoControllerRemote("mock", CONTROLLER_URL, targetFolder);
	}
	
	@Override
	@Test
	public void testPassRegularLifeCycle() {
		super.testPassRegularLifeCycle();
	}
	@Override
	@Test
	public void testPassWhenPreviousRunDidNotDeleteVideo() {
		super.testPassWhenPreviousRunDidNotDeleteVideo();
	}
	@Override
	@Test
	public void testPassWhenPreviousRunDidNotStopRecorder() {
		super.testPassWhenPreviousRunDidNotStopRecorder();
	}
	@Override
	@Test
	public void testFailOnStartBecauseRecorderDoesNotExist() {
		super.testFailOnStartBecauseRecorderDoesNotExist();
	}
	@Override
	@Test
	public void testFailOnStopBecauseCanNotCopyVideo() {
		super.testFailOnStopBecauseCanNotCopyVideo();
	}
	@Override
	@Test
	public void testFailOnStopBecauseCanNotStopRecorder() {
		super.testFailOnStopBecauseCanNotStopRecorder();
	}
	@Override
	@Test
	public void testContainerNotReadyAfterWaitForLogMessage() {
		super.testContainerNotReadyAfterWaitForLogMessage();
	}

}