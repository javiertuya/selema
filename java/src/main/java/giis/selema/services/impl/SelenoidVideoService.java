package giis.selema.services.impl;

import java.util.HashMap;
import java.util.Map;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.remote.RemoteWebDriver;

import giis.portable.util.FileUtil;
import giis.portable.util.JavaCs;
import giis.selema.services.IMediaContext;
import giis.selema.services.ISelemaLogger;
import giis.selema.services.IVideoService;

/**
 * Support for video recording from a selenoid service: Selenoid produces a video per each driver session,
 * this service keeps track of the video names, links to videos from the logs and timestamps when failures are produced
 */
public class SelenoidVideoService implements IVideoService {
	private ISelemaLogger log;
	private static final String VIDEO_INDEX_NAME = "video-index.log";
	protected String seleniumSessionId="";
	//los timestamps no se miden de forma precisa, pero se tomara como referencia el intervalo que se conoce
	private long lastSessionStartingTimestamp=0;
	private long lastSessionStartedTimestamp=0;
	
	/**
	 * Configures this service, called on attaching the service to a SeleManager
	 */
	@Override
	public IVideoService configure(ISelemaLogger thisLog) {
		log=thisLog;
		return this;
	}
	@Override
	public void beforeCreateDriver() {
		lastSessionStartingTimestamp=JavaCs.currentTimeMillis();
	}
	@Override
	public void afterCreateDriver(WebDriver driver) {
		if (driver instanceof RemoteWebDriver) // should be remote
			seleniumSessionId = ((RemoteWebDriver) driver).getSessionId().toString();
		lastSessionStartedTimestamp=JavaCs.currentTimeMillis();
	}
	@Override
	public String onTestFailure(IMediaContext context, String testName) {
		long nowTimestamp=JavaCs.currentTimeMillis();
		String videoFileName=context.getVideoFileName(testName);
		String videoUrl="<a href=\"" + videoFileName + "\">" + videoFileName + "</a>";
		String videoMsg="Recording video at " + getSessionTimestamp(nowTimestamp) + " (aprox.): " + videoUrl;
		if (log!=null)
			log.info(videoMsg);
		return videoMsg;
	}
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
	private String getSessionTimestamp(long nowTimestamp) {
		long starting=(nowTimestamp-lastSessionStartingTimestamp)/1000;
		long started=(nowTimestamp-lastSessionStartedTimestamp)/1000;
		return "[" + formatSeconds(started) + " " + formatSeconds(starting) + "]";
	}
	private String formatSeconds(long totalSeconds) {
		long seconds=totalSeconds % 60;
		long minutes=totalSeconds / 60;
		return (minutes<10 ?"0":"") + minutes+ ":" + (seconds<10 ?"0":"") + seconds;
	}
	@Override
	public void beforeQuitDriver(IMediaContext context, String testName) {
		String videoFileName=context.getVideoFileName(testName);
		videoFileName=getVideoFileNameWithRelativePath(videoFileName);
		if (log!=null)
			log.info("Saving video: " + "<a href=\"" + videoFileName + "\">" + videoFileName + "</a>");
		String videoIndex=FileUtil.getPath(context.getReportFolder(), VIDEO_INDEX_NAME);
		FileUtil.fileAppend(videoIndex, videoFileName + "\n");
	}

}
