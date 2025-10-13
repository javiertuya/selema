package giis.selema.services.impl;

import giis.selema.services.IBrowserService;
import giis.selema.services.IVideoController;
import giis.selema.services.IVideoService;

/**
 * This browser service video recording works differently than the Selenoid and Selenium dynamic grid, because here, the
 * browser server is composed by a preloaded browser server and a sidecar video server. The video server records video
 * while is active, that would create large videos including all selenium sessions.
 * 
 * To overcome this problem, when calling the setVideo method, this service receives an instance of IVideoController
 * that manages the start/stop of the video recording when the selenium session is open/closed.
 */
public class RemoteBrowserService extends AbstractBrowserService {

	private IVideoController videoController;

	@Override
	public IBrowserService setVideo() {
		throw new UnsupportedOperationException("RemoteBrowserService requires an IVideoController "
				+ "to start and stop video recording before and after the driver instantiation");
	}

	public IBrowserService setVideo(IVideoController videoController) {
		this.recordVideo = true;
		this.videoController = videoController;
		return this;
	}

	@Override
	public IVideoService getVideoRecorder() {
		return recordVideo ? new RemoteVideoService(videoController) : null;
	}

}
