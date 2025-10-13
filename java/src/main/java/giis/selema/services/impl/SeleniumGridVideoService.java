package giis.selema.services.impl;

import java.util.HashMap;
import java.util.Map;

import giis.portable.util.JavaCs;
import giis.selema.services.IMediaContext;

public class SeleniumGridVideoService extends AbstractVideoService {

	@Override
	protected String getVideoFileNameWithRelativePath(String videoFileName) {
		// I don't know why, but the recorded video names have a suffix mp4 before the .mp4 file name extension
		videoFileName = JavaCs.substring(videoFileName, 0, videoFileName.length() - 4) + "mp4.mp4";
		// Grid places each video under a different folder
		return "".equals(seleniumSessionId) ? seleniumSessionId : seleniumSessionId + "/" + videoFileName;
	}

	@Override
	public Map<String, Object> getSeleniumOptions(IMediaContext context, String testName) {
		String videoFileName = context.getVideoFileName(testName);
		Map<String, Object> opts = new HashMap<String, Object>(); // NOSONAR net compatibility
		opts.put("se:recordVideo", true);
		opts.put("se:name", videoFileName);
		return opts;
	}

}
