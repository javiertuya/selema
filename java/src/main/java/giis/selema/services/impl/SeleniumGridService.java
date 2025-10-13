package giis.selema.services.impl;

import java.util.HashMap;
import java.util.Map;

import giis.selema.services.IMediaContext;
import giis.selema.services.IVideoService;

public class SeleniumGridService extends SelenoidService {

	@Override
	public void addBrowserServiceOptions(Map<String, Object> allOptions, IVideoService videoRecorder,
			IMediaContext mediaVideoContext, String driverScope) {
		// Unlike selenoid, browser specific options in grid are not grouped
		allOptions.putAll(this.getSeleniumOptions(driverScope));
		if (videoRecorder != null)
			allOptions.putAll(videoRecorder.getSeleniumOptions(mediaVideoContext, driverScope));
	}

	@Override
	public IVideoService getVideoRecorder() {
		return recordVideo ? new SeleniumGridVideoService() : null;
	}

	@Override
	public Map<String, Object> getSeleniumOptions(String sessionName) {
		Map<String, Object> opts = new HashMap<String, Object>(); // NOSONAR net compatibility
		opts.put("se:vncEnabled", enableVnc); // can't be controlled ad driver instantiation?
		opts.putAll(specialCapabilities);
		return opts;
	}

}
