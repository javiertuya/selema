package giis.selema.manager;

import giis.selema.services.ISelemaLogger;
import giis.selema.services.impl.SelemaLogger;

/**
 * Creates an instance of the selema html logger
 */
public class LogFactory {
	//20-jan-2022 removed commented code on factory using standard logger (keep logConfigurator as txt)
	public ISelemaLogger getLogger(String loggerName, String reportDir, String logFileName) {
		return new SelemaLogger(loggerName, reportDir, logFileName);
	}
}
