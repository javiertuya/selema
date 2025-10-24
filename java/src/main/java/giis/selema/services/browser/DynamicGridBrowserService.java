package giis.selema.services.browser;

import java.util.HashMap;
import java.util.Map;

import giis.selema.services.IMediaContext;
import giis.selema.services.IVideoService;

public class DynamicGridBrowserService extends AbstractBrowserService {

	@Override
	public void addBrowserServiceOptions(Map<String, Object> allOptions, IVideoService videoRecorder,
			IMediaContext mediaVideoContext, String driverScope) {
		// Unlike services like selenoid, browser specific options in grid are not grouped
		allOptions.putAll(this.getSeleniumOptions(driverScope));
		if (videoRecorder != null)
			allOptions.putAll(videoRecorder.getSeleniumOptions(mediaVideoContext, driverScope));
	}

	@Override
	public IVideoService getNewVideoRecorder() {
		return recordVideo ? new DynamicGridVideoService() : null;
	}

	@Override
	protected Map<String, Object> getSeleniumOptions(String sessionName) { // NOSONAR not all methods will require sessionName
		Map<String, Object> opts = new HashMap<String, Object>(); // NOSONAR net compatibility
		opts.put("se:vncEnabled", enableVnc); // can't be controlled ad driver instantiation?
		opts.putAll(specialCapabilities);
		return opts;
	}

}
