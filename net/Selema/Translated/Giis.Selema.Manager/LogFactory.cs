/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Services;
using Giis.Selema.Services.Impl;
using Sharpen;

namespace Giis.Selema.Manager
{
	/// <summary>Creates an instance of the selema html logger</summary>
	public class LogFactory
	{
		//static final Logger log=LoggerFactory.getLogger(LogFactory.class);
		//private static Map<String, ISelemaLogger> loggers=new HashMap<>();
		public virtual ISelemaLogger GetLogger(string loggerName, string reportDir, string logFileName)
		{
			return new SelemaLogger(loggerName, reportDir, logFileName);
		}
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
