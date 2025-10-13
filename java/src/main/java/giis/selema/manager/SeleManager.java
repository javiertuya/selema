package giis.selema.manager;

import java.util.HashMap;
import java.util.Map;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.openqa.selenium.Capabilities;
import org.openqa.selenium.WebDriver;

import giis.portable.util.FileUtil;
import giis.portable.util.JavaCs;
import giis.selema.services.IBrowserService;
import giis.selema.services.ICiService;
import giis.selema.services.IJsCoverageService;
import giis.selema.services.IMediaContext;
import giis.selema.services.IScreenshotService;
import giis.selema.services.ISelemaLogger;
import giis.selema.services.ISoftAssertService;
import giis.selema.services.IVideoService;
import giis.selema.services.IVisualAssertService;
import giis.selema.services.IWatermarkService;
import giis.selema.services.impl.MediaContext;
import giis.selema.services.impl.ScreenshotService;
import giis.selema.services.impl.SoftAssertService;
import giis.selema.services.impl.VisualAssertService;

/**
 * Core Selema component that manages the appropriate actions in response to the test lifecycle events;
 * also contains methods for configuration and use during the test, 
 * see https://github.com/javiertuya/selema#readme for instructions
 */
public class SeleManager {
	final Logger log=LoggerFactory.getLogger(this.getClass()); //general purpose logger
	private ISelemaLogger selemaLog; //SeleManager html log
	
	private String currentBrowser="chrome";
	private String currentDriverUrl=""; //empty is local
	private Map<String, Object> currentOptions=null;
	private String[] currentArguments=null;
	private Capabilities currentOptionsInstance=null;
	private String driverVersion=DriverVersion.DEFAULT; // string with strategy to use or a given version
	
	//Currently managed driver
	private WebDriver currentDriver=null;
	//Current context, only used for external calls from tests as watermark or getting driver in unmanaged mode
	private String currentClassName="";
	private String currentTestName="";
	public String currentClassName() { return currentClassName; }
	public String currentTestName() { return currentTestName; }
		
	//Driver modes of operation
	private boolean manageAtTest=true;
	private boolean manageAtClass=false;

	private IMediaContext mediaVideoContext;
	private IMediaContext mediaScreenshotContext;
	private IMediaContext mediaDiffContext;
	private static int instanceCount=0; //uniquely identifies each instance of this class
	private int sessionCount=0; //uniquely identifies each session (new driver) created by this instance
	private boolean lastSessionRemote=false; //keeps track of the kind of session

	//Behaviour parameters
	private SelemaConfig conf=null;
	private IDriverConfigDelegate driverConfig=null;
	private IWatermarkService watermark=null;
	private ICiService ciService=null;
	private IScreenshotService screenshotService=null;
	private IVisualAssertService visualAssertService=null;
	private ISoftAssertService softAssertService=null;
	private IBrowserService browserService=null;
	private IVideoService videoRecorder=null;
    private IJsCoverageService coverageRecorder=null;
    private boolean maximizeOnCreate=false;

    /**
     * Creates an instance with the default configuration
     */
	public SeleManager() { 
		this(new SelemaConfig());
	}
	/**
	 * Creates an instance with a given configuration
	 */
	public SeleManager(SelemaConfig selemaConfig) { 
		if (selemaConfig==null)
			throw new SelemaException("SeleManager instance requires an instance of SelemaConfig");
		conf = selemaConfig;
		//ensures report folder is available
		FileUtil.createDirectory(conf.getReportDir());
		this.selemaLog=new LogFactory().getLogger(conf.getName(), conf.getReportDir(), conf.getLogName());
		
		//Attach predefined services
		ciService=new CiServiceFactory().getCurrent();
		screenshotService=new ScreenshotService().configure(selemaLog);
		visualAssertService=new VisualAssertService().configure(selemaLog, ciService.isLocal(), conf.getProjectRoot(), conf.getReportSubdir());
		softAssertService=new SoftAssertService().configure(selemaLog, ciService.isLocal(), conf.getProjectRoot(), conf.getReportSubdir());
		//Other services must be configuresd using add methods
		
		instanceCount++;
		selemaLog.info("*** Creating SeleManager instance " + instanceCount + " on " + ciService.getName());
	}
	
	/**
	 * Gets the current configuration used when instantiating this class
	 */
	public SelemaConfig getConfig() {
		return this.conf;
	}
	
	/**
	 * Executes the SeleManager configuration actions established by the IManagerConfig delegate passed as parameter
	 */
	public SeleManager setManagerDelegate(IManagerConfigDelegate configDelegate) {
		configDelegate.configure(this);
		return this;
	}

	/**
	 * Sets the WebDriver for the specified browser ("chrome","firefox","edge","safari","opera"), default is chrome
	 */
	public SeleManager setBrowser(String browser) {
		log.debug("Set browser: "+browser);
		this.currentBrowser=browser;
		return this;
	}
	/**
	 * Sets a RemoteWebDriver instead a local one (default); The driverUrl must point to the browser service
	 */
	public SeleManager setDriverUrl(String driverUrl) {
		log.debug("Set driver url: "+driverUrl);
		this.currentDriverUrl=driverUrl;
		return this;
	}
	/**
	 * Sets an object that can be used to provide additional configurations to the driver just after its creation
	 */
	public SeleManager setDriverDelegate(IDriverConfigDelegate driverConfig) {
		log.debug("Set driver Config instance");
		this.driverConfig=driverConfig;
		return this;
	}
	public String getDriverUrl() {
		return this.currentDriverUrl;
	}
	/**
	 * Sets the driver version selection strategy or the value of the desired driver version to set
	 */
	public SeleManager setDriverVersion(String driverVersion) {
		this.driverVersion = driverVersion;
		return this;
	}
	/**
	 * Adds the specific capabilities to the WebDriver prior to its creation
	 */
	public SeleManager setOptions(Map<String,Object> options) {
		log.debug("Set options: "+options.toString());
		this.currentOptions=options;
		return this;
	}
	/**
	 * Adds a browser dependent instance of options to set the W3C WebDriver standard capabilities.
	 * The capabilities specified with setOptions and setArguments
	 * will be added as well to the WebDriver prior to its creation
	 */
	public SeleManager setOptionsInstance(Capabilities optionsInstance) {
		log.debug("Set options instance: "+optionsInstance.getClass().getName());
		this.currentOptionsInstance=optionsInstance;
		return this;
	}
	/**
	 * Adds the specific arguments to the WebDriver execution
	 */
	public SeleManager setArguments(String[] arguments) {
		log.debug("Set arguments: "+JavaCs.deepToString(arguments));
		this.currentArguments=arguments;
		return this;
	}
	/**
	 * Starts the created drivers as maximized
	 */
	public SeleManager setMaximize(boolean doMaximize) {
		maximizeOnCreate=doMaximize;
		return this;
	}

	/**
	 * Returns to the default behaviour (a driver per each test)
	 */
	public SeleManager setManageAtTest() {
		log.debug("Set manage at test");
		this.manageAtTest=true;
		this.manageAtClass=false;
		return this;
	}
	/**
	 * Starts a WebDriver before the first test at each class, and quits after all tests in the class
	 */
	public SeleManager setManageAtClass() {
		log.debug("Set manage at class");
		this.manageAtTest=false;
		this.manageAtClass=true;
		return this;
	}
	/**
	 * Do not start/quit a webdriver, if needed, you can control the driver instantiation by calling the `createDriver()`
	 * and `quitDriver(WebDriver driver)` on the SeleManager Instance
	 */
	public SeleManager setManageNone() {
		log.debug("Set unmanaged");
		this.manageAtTest=false;
		this.manageAtClass=false;
		return this;
	}
	public boolean getManageAtTest() {
		return this.manageAtTest;
	}
	public boolean getManageAtClass() {
		return this.manageAtClass;
	}
	
	/** 
	 * Attaches a browser service (eg selenoid)
	 */
	public SeleManager add(IBrowserService browserSvc) {
		log.debug("Add browser service: " + browserSvc.getClass().getSimpleName());
		browserService=browserSvc;
		//when creating this service a compatible video recorder service is created too
		videoRecorder=browserService.getVideoRecorder();
		if (videoRecorder!=null)
			videoRecorder.configure(selemaLog);
		return this;
	}
	/** 
	 * Attaches a watermark service that inserts a text at the top left side of the browser with the name of test being executed and the failure status. 
	 */
	public SeleManager add(IWatermarkService watermark) {
		log.debug("Add watermark service");
		this.watermark=watermark;
		return this;
	}
	/** 
	 * Attaches a javascript coverage service
	 */
	public SeleManager add(IJsCoverageService recorder) {
		log.debug("Add js coverage service");
		coverageRecorder=recorder.configure(selemaLog, conf.getReportDir());
		return this;
	}
	
	public ISelemaLogger getLogger() {
		return this.selemaLog;
	}
	public ICiService getCiService() {
		return this.ciService;
	}
	public IScreenshotService getScreenshotService() {
		return this.screenshotService;
	}
	public IWatermarkService getWatermarkService() {
		return this.watermark;
	}
	public IJsCoverageService getCoverageService() {
		return this.coverageRecorder;
	}

	public boolean usesRemoteDriver() {
		return !JavaCs.isEmpty(currentDriverUrl);
	}
	
	//Driver management
	
	/**
	 * Gets the current driver managed, logs and throws exception is not set
	 */
	public WebDriver getDriver() {
		if (currentDriver==null) {
			selemaLog.error("The Selenium Manager does not have any active WebDriver");
			throw new SelemaException("The Selenium Manager does not have any active WebDriver");
		}
		return currentDriver; 
	}
	/**
	 * Gets the current driver managed, logs and throws exception is not set 
	 * (synonym of getDriver() accessed as a property on net)
	 */
	public WebDriver driver() {
		return getDriver(); 
	}
	/**
	 * Indicates if the web driver has been instantiated
	 */
	public boolean hasDriver() {
		return currentDriver!=null;
	}
    /**
	 * Gets a new WebDriver for the specified class and test. For inernal use only
	 */
	public WebDriver createDriver(String className, String testName) { 
		sessionCount++;
		mediaVideoContext=new MediaContext(conf.getReportDir(), conf.getQualifier(), instanceCount, sessionCount);
		mediaScreenshotContext=new MediaContext(conf.getReportDir(), conf.getQualifier(), instanceCount, sessionCount);
		mediaDiffContext=new MediaContext(conf.getReportDir(), conf.getQualifier(), instanceCount, sessionCount);
		if (this.usesRemoteDriver()) {
			lastSessionRemote=true;
			selemaLog.info("Remote session " + currentBrowser + " starting on " + currentDriverUrl + " ...");
			//Colecciona los datos para identificacion de nombre de sesion para visualizacion en selenoid-ui y nombrado de videos
			String driverScope=getDriverScope(className, testName);
			//Colecciona la informacion para localizar posteriormente el instante del fallo en los videos y obtiene el driver
			//El instante antes y despues de creacion del driver sera el margen de tolerancia de este instante
			if (videoRecorder!=null)
				videoRecorder.beforeCreateDriver();
			WebDriver rdriver=getRemoteSeleniumDriver(driverScope);
			if (videoRecorder!=null)
				videoRecorder.afterCreateDriver(rdriver);
			selemaLog.info("Remote session " + currentBrowser + " started. Scope: " + driverScope);
			currentDriver=rdriver;
		} else {
			lastSessionRemote=false;
			String args="";
			selemaLog.info("Local session " + currentBrowser + " starting" + args + " ...");
			currentDriver=getLocalSeleniumDriver();
		}
		if (coverageRecorder!=null)
			coverageRecorder.afterCreateDriver(currentDriver);
		if (maximizeOnCreate)
			currentDriver.manage().window().maximize();
		if (driverConfig!=null)
			driverConfig.configure(currentDriver);
		return currentDriver;
	}
    /**
	 * Gets a new WebDriver for the current class and test, use when running in unmanaged mode
	 */
	public WebDriver createDriver() { 
		return createDriver(currentClassName, currentTestName);
	}
    /**
	 * Quits the WebDriver for the specified class and test. For inernal use only
	 */
	public WebDriver quitDriver(WebDriver driver, String className, String testName) { //NOSONAR Methods returns should not be invariant: here the goal is to reset the driver variable
		if (driver==null)
			return null;
		if (coverageRecorder!=null)
			coverageRecorder.beforeQuitDriver(driver);
		if (videoRecorder!=null && lastSessionRemote) {
			videoRecorder.beforeQuitDriver(mediaVideoContext, getDriverScope(className, testName));
			selemaLog.info("Remote session ending");
		}
		driver.quit();
		currentDriver=null;
		return currentDriver; //NOSONAR
	}
    /**
	 * Quits the WebDriver for the current class and test, use when running in unmanaged mode
	 */
	public WebDriver quitDriver(WebDriver driver) {
		return quitDriver(driver, currentClassName, currentTestName);
	}
    /**
	 * Replaces the WebDriver for the specified class and test by the specified. For inernal use only
	 */
	public void replaceDriver(WebDriver driver) {
		currentDriver=driver;
	}

	//Response to lifecycle events
	
	public void onSetUpClass(String className) {
		log.trace("on set up class "+className);
		currentClassName=className;
		currentTestName="";
		if (manageAtClass)
			createDriver(className, "");
	}
	public void onSetUp(String className, String testName) {
		log.trace("on set up test "+testName);
		selemaLog.info("*** SetUp - " + testName);
		currentClassName=className;
		currentTestName=testName;
		if (manageAtTest) {
			createDriver(className, testName);
		}
	}
	public void onTearDown(String className, String testName) {
		log.trace("on tear down test "+testName);
		selemaLog.info("TearDown - " + testName);
		if (manageAtTest)
			quitDriver(currentDriver, className, testName); 
	}
	public void onTearDownClass(String className, String testName) {
		log.trace("on tear down class "+className);
		if (manageAtClass)
			quitDriver(currentDriver, className, testName);
	}
	public String onFailure(String className, String testName) {
		log.trace("on test failure "+testName);
		selemaLog.warn("FAIL " + testName);
		String msg="";
		if (currentDriver!=null)
			msg+=screenshotService.takeScreenshot(currentDriver, mediaScreenshotContext, testName);
		if (videoRecorder!=null && lastSessionRemote)
			msg+=videoRecorder.onTestFailure(mediaVideoContext, getDriverScope(className, testName));
		//despues de screenshot y videos por si interfiere en el estado de la pantalla o el instante del fallo
		if (currentDriver!=null && watermark!=null) 
			msg+=watermarkFail(currentDriver, testName);
		return msg;
	}
	// After update to Selenium 4.11 we detected some tests that failed with "invalid session id" message.
	// This was caused by button cliks using javascript executor that apparently crashed the browser
	// and cause a fist failure in the watermark writing that is controlled and logged now
	private String watermarkFail(WebDriver driver, String value) {
		try {
			watermark.fail(driver, value);
			return "";
		} catch (RuntimeException e) {
			String msg="Can't write onFailure watermark " + value + ". Message: " + e.getMessage();
			log.error(msg);
			selemaLog.error(msg);
			return msg;
		}
	}
	public void onSuccess(String testName) {
		log.trace("on test success "+testName);
		selemaLog.info("SUCCESS " + testName);
	}
	private String getDriverScope(String className, String testName) {
		String scope = manageAtClass ? className : testName;
		//Le anyade el id del job para evitar mezclar videos cuando se comparte la instancia de selenoid
		scope += " " + this.getCiService().getJobId();
		return scope;
	}

	//Proxies to some common services
	
	/**
	 * Takes a screenshot to a file that will be linked to the log
	 */
	public void screenshot(String fileName) {
		screenshotService.takeScreenshot(this.driver(), mediaScreenshotContext, fileName);
	}
	/**
	 * Places a watermark with the test name (requires the watermark service be attached)
	 */
	public void watermark() {
		watermarkText(currentTestName);
	}
	/**
	 * Places a watermark with a given text (requires the watermark service be attached)
	 */
	public void watermarkText(String value) {
		if (watermark==null) {
			selemaLog.error("Watermark service is not attached to this Selenium Manager");
			throw new SelemaException("Watermark service is not attached to this Selenium Manager");
		}
		try {
			watermark.write(this.driver(), value);
		} catch (RuntimeException e) {
			String msg="Can't write onFailure watermark " + value + ". Message: " + e.getMessage();
			log.warn(msg);
			selemaLog.warn(msg);
		}
	}
	/**
	 * Asserts if two large strings and links the html differences to the log
	 */
	public void visualAssertEquals(String expected, String actual) {
		visualAssertEquals(expected, actual, "");
	}
	/**
	 * Asserts if two large strings and links the html differences to the log, including an additional message
	 */
	public void visualAssertEquals(String expected, String actual, String message) {
		visualAssertService.assertEquals(expected, actual, message, mediaDiffContext, currentTestName);
	}

	/**
	 * Soft Asserts if two large strings and links the html differences to the log:
	 * records the assertion message instead of throwing and exception until softAssertAll() is called
	 */
	public void softAssertEquals(String expected, String actual) {
		softAssertEquals(expected, actual, "");
	}
	/**
	 * Soft Asserts if two large strings and links the html differences to the log with an additonal message:
	 * records the assertion message instead of throwing and exception until softAssertAll() is called
	 */
	public void softAssertEquals(String expected, String actual, String message) {
		softAssertService.assertEquals(expected, actual, message, mediaDiffContext, currentTestName);
	}
	/**
	 * Throws and exception if at least one soft assertion failed including all assertion messages
	 */
	public void softAssertAll() {
		softAssertService.assertAll();
	}
	/**
	 * Resets the current soft assertion failure messages that are stored
	 */
	public void softAssertClear() {
		softAssertService.assertClear();
	}
	
	private WebDriver getLocalSeleniumDriver() { 
		log.trace("Get local Selenium Driver");
		return new SeleniumDriverFactory().getSeleniumDriver(currentBrowser, "", this.driverVersion, currentOptions, currentArguments, currentOptionsInstance);
	}
	private WebDriver getRemoteSeleniumDriver(String driverScope) {
		log.trace("Get remote Selenium Driver");
		//prepara las opciones anyadiendo a las definidas al configurar, las requeridas por los diferentes servicios
		Map<String, Object> allOptions = new HashMap<String, Object>(); // NOSONAR net compatibility
		if (currentOptions!=null)
			allOptions.putAll(currentOptions);
		if (browserService!=null)
			browserService.addBrowserServiceOptions(allOptions, videoRecorder, mediaVideoContext, driverScope);
		
		return new SeleniumDriverFactory().getSeleniumDriver(currentBrowser, currentDriverUrl, driverVersion, allOptions, currentArguments, currentOptionsInstance);
	}
	
}
