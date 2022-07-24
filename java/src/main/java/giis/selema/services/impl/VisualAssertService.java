package giis.selema.services.impl;

import giis.selema.portable.FileUtil;
import giis.selema.services.IMediaContext;
import giis.selema.services.ISelemaLogger;
import giis.selema.services.IVisualAssertService;
import giis.visualassert.VisualAssert;

/**
 * Management of VisualAssert comparisons between long strings, managing the html diff files and links from the log
 */
public class VisualAssertService implements IVisualAssertService {
	private VisualAssert va;
	private ISelemaLogger logger;
	
	public VisualAssertService() {
	    this.va = new VisualAssert();
	}
	/**
	 * Configures this service, called on attaching the service to a SeleniumManager
	 */
	@Override
	public IVisualAssertService configure(ISelemaLogger logger, boolean local, String projectRoot, String reportSubdir) {
	    this.logger=logger;
	    this.va.setUseLocalAbsolutePath(local).setReportSubdir(FileUtil.getPath(projectRoot, reportSubdir));
		return this;
	}
	/**
	 * General assertion and log writing, intended to be used from a proxy in the SeleniumManager class
	 */
	@Override
	public void assertEquals(String expected, String actual, String message, IMediaContext context, String testName) {
		if (!expected.equals(actual)) {
			if (logger!=null && context!=null) {
				//before assert determines file name and create log
				String diffFile=context.getDiffFileName(testName);
				String diffUrl="<a href=\"" + diffFile + "\">" + diffFile + "</a>";
				logger.warn("Visual Assert differences: " + diffUrl + ("".equals(message) ? "" : " - Message: " + message));
				va.assertEquals(expected, actual, message, diffFile);
			} else //si no hay contexto ejecuta directametnte el assert y que genere el archivo
				va.assertEquals(expected, actual, message, "");
		}
	}
}
