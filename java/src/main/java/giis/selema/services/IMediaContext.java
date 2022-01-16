package giis.selema.services;

/**
 * Manages the filenames of media files produced durint testing (screenshots, videos, diff files)
 */
public interface IMediaContext {

	String getReportFolder();

	String getScreenshotFileName(String testName);

	String getVideoFileName(String testName);

	String getDiffFileName(String testName);

}