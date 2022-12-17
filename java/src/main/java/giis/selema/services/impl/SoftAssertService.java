package giis.selema.services.impl;

import giis.selema.portable.FileUtil;
import giis.selema.services.IMediaContext;
import giis.selema.services.ISelemaLogger;
import giis.selema.services.ISoftAssertService;
import giis.visualassert.SoftVisualAssert;

/**
 * Management of SoftVisualAssert comparisons between long strings, managing the html diff files and links from the log
 */
public class SoftAssertService implements ISoftAssertService {
	private SoftVisualAssert sva;
	private ISelemaLogger logger;
	
	public SoftAssertService() {
	    this.sva = new SoftVisualAssert();
	}
	/**
	 * Configures this service, called on attaching the service to a SeleManager
	 */
	@Override
	public ISoftAssertService configure(ISelemaLogger logger, boolean local, String projectRoot, String reportSubdir) {
	    this.logger=logger;
	    this.sva.setUseLocalAbsolutePath(local).setReportSubdir(FileUtil.getPath(projectRoot, reportSubdir));
		return this;
	}
	/**
	 * General soft assertion and log writing, intended to be used from a proxy in the SeleManager class
	 */
	@Override
	public void assertEquals(String expected, String actual, String message, IMediaContext context, String testName) {
		if (!expected.equals(actual)) {
			if (logger!=null && context!=null) {
				//before assert determines file name and create log
				String diffFile=context.getDiffFileName(testName);
				String diffUrl="<a href=\"" + diffFile + "\">" + diffFile + "</a>";
				logger.warn("Soft Visual Assert differences (Failure " + (sva.getFailureCount()+1) + "): " 
						+ diffUrl + ("".equals(message) ? "" : " - Message: " + message));
				sva.assertEquals(expected, actual, message, diffFile);
			} else //si no hay contexto ejecuta directametnte el assert y que genere el archivo
				sva.assertEquals(expected, actual, message, "");
		}
	}
	
	/**
	 * Throws and exception if at least one assertion failed including all assertion messages
	 */
	@Override
	public void assertAll() {
		sva.assertAll();
	}
	
	/**
	 * Resets the current failure messages that are stored
	 */
	@Override
	public void assertClear() {
		sva.assertClear();
	}

}
