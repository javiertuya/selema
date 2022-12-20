package giis.selema.services.impl;

import org.openqa.selenium.WebDriver;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.portable.FileUtil;
import giis.selema.portable.selenium.SeleniumActions;
import giis.selema.services.IJsCoverageService;
import giis.selema.services.ISelemaLogger;

/**
 * Support for JSCover javascript coverage.
 */
public class JsCoverService implements IJsCoverageService {
	final Logger log=LoggerFactory.getLogger(this.getClass());
	private ISelemaLogger selemaLog;
	//html to reset browser local memory
	private static final String CLEAR_LOCAL_STORAGE_HTML="/jscoverage-clear-local-storage.html";
	//html to restore browser local memory from file
	private static final String RESTORE_LOCAL_STORAGE_HTML="/jscoverage-restore-local-storage.html";
	//js script executed to save coverage from browser local memory to file
	private static final String SAVE_COVERAGE_SCRIPT="return jscoverage_serializeCoverageToJSON();";
	//js script to restore coverage from file
	private static final String RESTORE_COVERAGE_SCRIPT="restoreLocalStorage()";

	private String appRoot;
	private String reportDir;
	private String savedCoverageFile;
	private boolean resetDone=false;

	//This shall be instantiated as a singleton
	private static JsCoverService instance;
	private JsCoverService(String appRootUrl) {
		this.appRoot=appRootUrl;
		this.savedCoverageFile="jscoverage.json";
	}
	/**
	 * Instantiation of this service as a singleton
	 */
	public static JsCoverService getInstance(String appRootUrl) {
		if (instance==null)
			instance=new JsCoverService(appRootUrl);
		return instance;	
	}
	
	/**
	 * Configures this service, called on attaching the service to a SeleManager
	 */
	@Override 
	public IJsCoverageService configure(ISelemaLogger thisLog, String reportDir) {
		this.selemaLog=thisLog;
		this.reportDir=reportDir;
		FileUtil.createDirectory(reportDir); //ensures the report folder exists
		return this;
	}
	/**
	 * Resets coverage stored in browser local memory and restores coverage from previous sessions.
	 * Reset is made calling an html file created by the JSCover instrumentation, executed only the first time.
	 * Subsequent executions call an additional html file to restore coverage from previous executions.
	 */
	@Override
	public void afterCreateDriver(WebDriver driver) {
		String coverageFileName=FileUtil.getPath(this.reportDir, savedCoverageFile); 
		if (!resetDone) {
			driver.get(appRoot + CLEAR_LOCAL_STORAGE_HTML);
			//Creates a first default file without coverage data
			//(avoids reporting failures if execution does not produces any coverage data)
			String defaultCoverage="{\"/EMPTY-JS-COVERAGE.js\":{\"lineData\":[null,0,0],\"functionData\":[0],\"branchData\":{}}}";
			FileUtil.fileWrite(coverageFileName, defaultCoverage);
		} else {
			driver.get(appRoot + RESTORE_LOCAL_STORAGE_HTML);
			//reads from file the previous coverage data and sends to browser local memory
			try {
				String coverage=FileUtil.fileRead(coverageFileName);
				SeleniumActions.setInnerText(driver, "coverageToRestore", coverage);
				SeleniumActions.executeScript(driver, RESTORE_COVERAGE_SCRIPT);
			} catch (RuntimeException e) {
				selemaLog.error("Exception reading js coverage file: " + e.getMessage());
			}
		}
		resetDone=true;
	}
	/**
	 * Saves coverage from browser local memory to the json external file jscoverage.json
	 */
	@Override
	public void beforeQuitDriver(WebDriver driver) {
		String coverageFileName=FileUtil.getPath(reportDir, savedCoverageFile); 
		try {
			selemaLog.debug("Saving JSCover coverage from browser local storage to file "+coverageFileName);
			String jsonCoverage=SeleniumActions.executeScript(driver, SAVE_COVERAGE_SCRIPT);
			selemaLog.trace("Content: "+jsonCoverage);
			FileUtil.fileWrite(coverageFileName, jsonCoverage);
		} catch (RuntimeException e) {
			selemaLog.error("Can't get JSCover coverage, either code is not instrumented or output file "+coverageFileName+" can't be saved: " + e.getMessage());
		}
	}
	/**
	 * Sets the file name where coverage is saved, only for testing
	 */
	public void setCoverageFileName(String name) {
		this.savedCoverageFile=name;
	}

}
