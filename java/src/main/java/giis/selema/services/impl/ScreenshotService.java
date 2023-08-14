package giis.selema.services.impl;

import org.openqa.selenium.WebDriver;

import giis.portable.util.FileUtil;
import giis.selema.portable.selenium.SeleniumActions;
import giis.selema.services.IMediaContext;
import giis.selema.services.IScreenshotService;
import giis.selema.services.ISelemaLogger;

/**
 * Screenshot management
 */
public class ScreenshotService implements IScreenshotService {
	private ISelemaLogger log;

	/**
	 * Configures this service, called on attaching the service to a SeleManager
	 */
	@Override
	public IScreenshotService configure(ISelemaLogger thisLog) {
	    log=thisLog;
	    return this;
	}
	/**
	 * Takes an picture of the current state of the browser and saves it to the reports folder
	 */
	@Override
	public String takeScreenshot(WebDriver driver, IMediaContext context, String testName) {
		String screenshotFile=context.getScreenshotFileName(testName);
		try {
			String fileName=FileUtil.getPath(context.getReportFolder(), screenshotFile);
			String screenshotUrl="<a href=\"" + screenshotFile + "\">" + screenshotFile + "</a>";
			String msg="Taking screenshot: " + screenshotUrl;
			if (log!=null)
				log.info(msg);
			SeleniumActions.takeScreenshotToFile(driver, fileName);
			return msg;
		} catch (RuntimeException e) {
			String msg="Can't take screenshot or write the content to file "+screenshotFile+". Message: " + e.getMessage();
			if (log!=null)
				log.error(msg.replace("\n", "<br/>")); //some messages may have more than one line
			return msg;
		}
	}

}

