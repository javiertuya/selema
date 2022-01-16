package giis.selema.services.impl;

import java.util.HashMap;
import java.util.Map;

import org.openqa.selenium.remote.RemoteWebDriver;

import giis.selema.portable.FileUtil;
import giis.selema.portable.JavaCs;
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
	//los timestamps no se miden de forma precisa, pero se tomara como referencia el intervalo que se conoce
	private long lastSessionStartingTimestamp=0;
	private long lastSessionStartedTimestamp=0;
	
	/**
	 * Configures this service, called on attaching the service to a SeleniumManager
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
	public void afterCreateDriver(RemoteWebDriver driver) {
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
	@Override
	public Map<String, Object> getSeleniumOptions(IMediaContext context, String testName) {
		String videoFileName=context.getVideoFileName(testName);
		HashMap<String, Object> opts= new HashMap<>();
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
		if (log!=null)
			log.info("Saving video: " + "<a href=\"" + videoFileName + "\">" + videoFileName + "</a>");
		String videoIndex=FileUtil.getPath(context.getReportFolder(), VIDEO_INDEX_NAME);
		FileUtil.fileAppend(videoIndex, videoFileName + "\n");
	}

}
