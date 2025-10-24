package giis.selema.services.impl;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.portable.util.FileUtil;
import giis.portable.util.JavaCs;
import giis.selema.services.ISelemaLogger;

/**
 * Custom logging to produce the Selema html log file that emulates the output produced by standard loggers.
 * Not using standard loggers to avoid interferences, but also issues calls to the current logger
 */
public class SelemaLogger implements ISelemaLogger {
	private static final String SPAN_HTML = "<br/><span style=\"font-family:monospace;\">";
	private static final String SPAN_RED = "<span style=\"color:red;\">";
	private static final String SPAN_RED_BOLD = "<span style=\"color:red;font-weight:bold;\">";
	private static final String SPAN_END = "</span>";
	private final Logger log;
	private String loggerName;
	private String reportDir;
	private String logFile;
	public SelemaLogger(String loggerName, String reportDir, String logFileName) {
		this.log=LoggerFactory.getLogger(loggerName);
		this.loggerName=loggerName;
		this.reportDir=reportDir;
		this.logFile=FileUtil.getPath(this.reportDir, logFileName);
		//ensures there is a directory for the logger
		FileUtil.createDirectory(reportDir);
		info("Logger file created at: " + logFile);
	}
	@Override
	public void trace(String message) {
		log.trace(message);
	}
	@Override
	public void debug(String message) {
		log.debug(message);
	}
	@Override
	public void info(String message) {
		write("INFO", message);
		log.info(message);
	}
	@Override
	public void warn(String message) {
		write("WARN", SPAN_RED + message + SPAN_END);
		log.warn(message);
	}
	@Override
	public void error(String message) {
		write("ERROR", SPAN_RED_BOLD + message + SPAN_END);
		log.error(message);
	}
	
	//only for testing
	public static String replaceTags(String logLine) {
		return logLine.replace(SPAN_HTML, "")
				.replace(SPAN_RED_BOLD, "")
				.replace(SPAN_RED, "")
				.replace(SPAN_END, "");
	}
	
	private void write(String type, String message) {
		String time=JavaCs.getTime(JavaCs.getCurrentDate());
		// some messages may have more than one line, replace line break by html break
		message = message.replace("\r", "").replace("\n", "<br/>");
		FileUtil.fileAppend(logFile, "\n" + html("[" + type + "] " + time + " " + loggerName + " - "+ message));
	}
	private String html(String message) {
		return SPAN_HTML + message + SPAN_END;
	}
}
