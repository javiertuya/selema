package giis.selema.services.impl;

import giis.portable.util.JavaCs;
import giis.selema.services.IMediaContext;

/**
 * Manages the filenames of media files produced durint testing (screenshots, videos, diff files)
 */
public class MediaContext implements IMediaContext {
	private String reportFolder;
	private String qualifier;
	private int instanceCount; //uniquely dentifies each SeleManager instance
	private int sessionCount; //uniquely identifies each new driver session created by this instance
	private int inSessionCount; //given a session, gets unique identifiers, if needed

	public MediaContext(String reportFolder, String qualifier, int instanceCount, int sessionCount) {
		this.reportFolder=reportFolder;
		this.qualifier=qualifier;
		this.instanceCount=instanceCount;
		this.sessionCount=sessionCount;
		this.inSessionCount=0;
	}
	@Override
	public String getReportFolder() {
		return reportFolder;
	}
	@Override
	public String getScreenshotFileName(String testName) {
		return "screen" + qualifier + "-" + getMediaFileName(testName,true) + ".png";
	}
	@Override
	public String getVideoFileName(String testName) {
		return "video" + qualifier + "-" + getMediaFileName(testName,false) + ".mp4";
	}
	@Override
	public String getDiffFileName(String testName) {
		return "diff" + qualifier + "-" + getMediaFileName(testName,true) + ".html";
	}
	private String getMediaFileName(String testName, boolean includeInSessionCount) {
		String prefix=doubleDigit(instanceCount) + "-" + doubleDigit(sessionCount) + "-";
		if (includeInSessionCount) {
			inSessionCount++;
			prefix=prefix + doubleDigit(inSessionCount) + "-";
		}
		return prefix + reprocessFileName(testName);
	}
	private String doubleDigit(long value) {
		return (value<10 ? "0" : "") + String.valueOf(value);
	}
	/**
	 * Removes all no alphanumerich characters to representa filename (without extension)
	 */
	public static String reprocessFileName(String name) {
		if (name==null || "".equals(name))
			return "noname";
		//sharpen in windows replaces non ascii (like enye) correctly
		//but in linux container replaces them to ??, make this first replacement to - in order to maintain compatibility between platforms
		String replaced=name.replace("??", "-");
		replaced=JavaCs.replaceRegex(replaced,"[^A-Za-z0-9]", "-");
		return replaced.length()>100 ? JavaCs.substring(replaced,0,100) : replaced;
	}

}
