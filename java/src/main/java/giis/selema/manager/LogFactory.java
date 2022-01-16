package giis.selema.manager;

import giis.selema.services.ISelemaLogger;
import giis.selema.services.impl.SelemaLogger;

/**
 * Creates an instance of the selema html logger
 */
public class LogFactory {
	//static final Logger log=LoggerFactory.getLogger(LogFactory.class);
	//private static Map<String, ISelemaLogger> loggers=new HashMap<>();
	
	public ISelemaLogger getLogger(String loggerName, String reportDir, String logFileName) {
		return new SelemaLogger(loggerName, reportDir, logFileName);
		/*
		if (loggers.containsKey(loggerName)) {
			log.debug("Using existing logger configuration: " + loggerName);
			return loggers.get(loggerName);
		} else {
			log.debug("Creating new logger configuration: " + loggerName);
			ISelemaLogger newLogger=new SelemaLogger(loggerName, reportDir, logFileName);
			loggers.put(loggerName, newLogger);
			return newLogger;
		}
		*/
	}
}
