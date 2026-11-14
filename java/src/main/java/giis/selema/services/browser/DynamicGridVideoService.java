package giis.selema.services.browser;

import java.util.HashMap;
import java.util.Map;

import giis.portable.util.JavaCs;
import giis.selema.services.IMediaContext;

public class DynamicGridVideoService extends AbstractVideoService {

	@Override
	protected String getVideoFileNameWithRelativePath(IMediaContext context, String testName) {
		String videoFileName = super.getVideoFileNameWithRelativePath(context, testName);
		// Grid places each video under a folder named as the session id, inside the folder mounted at /opt/selenium/assets.
		// To make the link resolve from the selema log, this folder must be mounted at the selema report folder
		return "".equals(seleniumSessionId) ? seleniumSessionId : seleniumSessionId + "/" + videoFileName;
	}

	@Override
	public Map<String, Object> getSeleniumOptions(IMediaContext context, String testName) {
		// Grid names the video as the se:name capability plus the .mp4 extension, after removing the characters
		// other than letters, digits, hyphens and underscores (the dot of the extension would be removed)
		String videoFileName = context.getVideoFileName(testName);
		String videoName = JavaCs.substring(videoFileName, 0, videoFileName.length() - ".mp4".length());
		Map<String, Object> opts = new HashMap<String, Object>(); // NOSONAR net compatibility
		opts.put("se:recordVideo", true);
		opts.put("se:name", videoName);
		return opts;
	}

}
