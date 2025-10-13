package giis.selema.services.impl;

import java.util.HashMap;
import java.util.Map;

import giis.selema.services.IMediaContext;

/**
 * Support for video recording from a selenoid service: Selenoid produces a video per each driver session,
 * this service keeps track of the video names, links to videos from the logs and timestamps when failures are produced
 */
public class SelenoidVideoService extends AbstractVideoService {
	
	@Override
	protected String getVideoFileNameWithRelativePath(String videoFileName) {
		return videoFileName; // video file strucutre is flat
	}
	
	@Override
	public Map<String, Object> getSeleniumOptions(IMediaContext context, String testName) {
		String videoFileName=context.getVideoFileName(testName);
		Map<String, Object> opts = new HashMap<String, Object>(); // NOSONAR net compatibility
		opts.put("enableVideo", true);
		opts.put("videoName", videoFileName);			
		return opts;
	}

}
