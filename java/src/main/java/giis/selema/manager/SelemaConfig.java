package giis.selema.manager;

import giis.selema.portable.FileUtil;
import giis.selema.portable.Parameters;

/**
 * Static configuration parameters at the instantiation of a SeleniumManager (location, name, etc)
 */
public class SelemaConfig { //NOSONAR
	
	private static final String DEFAULT_NAME="selema";
	//Path to the project root (depends on the platform)
	private String projectRoot=Parameters.DEFAULT_PROJECT_ROOT;
	//Path to the selema reports folder relative to projectRoot (depends on the platform)
	private String reportSubdir=Parameters.DEFAULT_REPORT_SUBDIR;
	
	private String logName=DEFAULT_NAME;

	/** 
	 * Gets the platform name (java or net), useful if conditional execution of tests is needed
	 */
	public String getPlatformName() {
		return Parameters.PLATFORM_NAME;
	}
	/**
	 * True if running in java platform
	 */
	public boolean isJava() {
		return "java".equals(getPlatformName());
	}
	/**
	 * True if running in net platform
	 */
	public boolean isNet() {
		return "net".equals(getPlatformName());
	}
	/**
	 * Gets the project root currently configured
	 */
	public String getProjectRoot() {
		return projectRoot;
	}
	/**
	 * Changes the location of the project root; 
	 * Default is current folder on Java and four levels up on NET 
	 * (this is the solution folder provided that the test project is located just below the solution folder)
	 */
	public SelemaConfig setProjectRoot(String root) {
		projectRoot=root;
		return this;
	}
	/**
	 * Gets the selema report folder currently configured
	 */
	public String getReportSubdir() {
		return reportSubdir;
	}
	/**
	 * Changes the name of the report folder (relative to the project root). Default is `target` (on Java) and `reports` (on .NET).
	 */
	public SelemaConfig setReportSubdir(String subdir) {
		reportSubdir=subdir;
		return this;
	}
	public String getReportDir() {
		return FileUtil.getPath(projectRoot, reportSubdir);
	}
	public String getName() {
		return logName;
	}
	/**
	 * Sets the name of the SeleniumManager (default is selema), useful when logs from different instances must be kept separated
	 */
	public SelemaConfig setName(String name) {
		logName=name;
		return this;
	}
	public String getQualifier() {
		return logName.equals(DEFAULT_NAME) ? "" : "-" + logName;
	}
	public String getLogName() {
		return "selema-log" + getQualifier() + ".html";
	}
	
}
