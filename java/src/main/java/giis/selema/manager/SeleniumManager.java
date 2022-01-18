package giis.selema.manager;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.remote.RemoteWebDriver;

import giis.selema.portable.FileUtil;
import giis.selema.portable.JavaCs;
import giis.selema.portable.SelemaException;
import giis.selema.services.IBrowserService;
import giis.selema.services.ICiService;
import giis.selema.services.IJsCoverageService;
import giis.selema.services.IMediaContext;
import giis.selema.services.IScreenshotService;
import giis.selema.services.ISelemaLogger;
import giis.selema.services.IVideoService;
import giis.selema.services.IVisualAssertService;
import giis.selema.services.IWatermarkService;
import giis.selema.services.impl.MediaContext;
import giis.selema.services.impl.ScreenshotService;
import giis.selema.services.impl.VisualAssertService;

/**
 * Core Selema component that manages the appropriate actions in response to the test lifecycle events;
 * also contains methods for configuration and use during the test, 
 * see https://github.com/javiertuya/selema#readme for instructions
 */
public class SeleniumManager {
	final Logger log=LoggerFactory.getLogger(this.getClass()); //general purpose logger
	private ISelemaLogger selemaLog; //SeleniumManager html log
	
	private String currentBrowser="chrome";
	private String currentDriverUrl=""; //empty is local
	private Map<String, Object> currentOptions=null;
	private String[] currentArguments=null;
	
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
	private List<String> driversWithSetupDone=new ArrayList<>(); //to avoid duplicate downloads of local drivers

	//Behaviour parameters
	private SelemaConfig conf=null;
	private IDriverConfigDelegate driverConfig=null;
	private IWatermarkService watermark=null;
	private ICiService ciService=null;
	private IScreenshotService screenshotService=null;
	private IVisualAssertService visualAssertService=null;
	private IBrowserService browserService=null;
	private IVideoService videoRecorder=null;
    private IJsCoverageService coverageRecorder=null;
    private boolean maximizeOnCreate=false;

    /**
     * Creates an instance with the default configuration
     */
	public SeleniumManager() { 
		this(new SelemaConfig());
	}
	/**
	 * Creates an instance with a given configuration
	 */
	public SeleniumManager(SelemaConfig selemaConfig) { 
		if (selemaConfig==null)
			throw new SelemaException("SeleniumManager instance requires an instance of SelemaConfig");
		conf = selemaConfig;
		//ensures report folder is available
		FileUtil.createDirectory(conf.getReportDir());
		this.selemaLog=new LogFactory().getLogger(conf.getName(), conf.getReportDir(), conf.getLogName());
		
		//Attach predefined services
		ciService=new CiServiceFactory().getCurrent();
		screenshotService=new ScreenshotService().configure(selemaLog);
		visualAssertService=new VisualAssertService().configure(selemaLog, ciService.isLocal(), conf.getProjectRoot(), conf.getReportSubdir());
		//Other services must be configuresd using add methods
		
		instanceCount++;
		selemaLog.info("*** Creating SeleniumManager instance " + instanceCount + " on " + ciService.getName());
	}
	
	/**
	 * Gets the current configuration used when instantiating this class
	 */
	public SelemaConfig getConfig() {
		return this.conf;
	}
	
	/**
	 * Executes the SeleniumManager configuration actions established by the IManagerConfig delegate passed as parameter
	 */
	public SeleniumManager setManagerDelegate(IManagerConfigDelegate configDelegate) {
		configDelegate.configure(this);
		return this;
	}

	/**
	 * Sets the WebDriver for the specified browser ("chrome","firefox","edge","safari","opera"), default is chrome
	 */
	public SeleniumManager setBrowser(String browser) {
		log.debug("Set browser: "+browser);
		this.currentBrowser=browser;
		return this;
	}
	/**
	 * Sets a RemoteWebDriver instead a local one (default); The driverUrl must point to the browser service
	 */
	public SeleniumManager setDriverUrl(String driverUrl) {
		log.debug("Set driver url: "+driverUrl);
		this.currentDriverUrl=driverUrl;
		return this;
	}
	/**
	 * Sets an object that can be used to provide additional configurations to the driver just after its creation
	 */
	public SeleniumManager setDriverDelegate(IDriverConfigDelegate driverConfig) {
		log.debug("Set driver Config instance");
		this.driverConfig=driverConfig;
		return this;
	}
	public String getDriverUrl() {
		return this.currentDriverUrl;
	}
	/**
	 * Adds the specific capabilities to the WebDriver prior to its creation
	 */
	public void setOptions(Map<String,Object> options) {
		log.debug("Set options: "+options.toString());
		this.currentOptions=options;
	}
	/**
	 * Adds the specific arguments to the WebDriver execution
	 */
	public void setArguments(String[] arguments) {
		log.debug("Set arguments: "+JavaCs.deepToString(arguments));
		this.currentArguments=arguments;
	}
	/**
	 * Starts the created drivers as maximized
	 */
	public SeleniumManager setMaximize(boolean doMaximize) {
		maximizeOnCreate=doMaximize;
		return this;
	}

	/**
	 * Returns to the default behaviour (a driver per each test)
	 */
	public SeleniumManager setManageAtTest() {
		log.debug("Set manage at test");
		this.manageAtTest=true;
		this.manageAtClass=false;
		return this;
	}
	/**
	 * Starts a WebDriver before the first test at each class, and quits after all tests in the class
	 */
	public SeleniumManager setManageAtClass() {
		log.debug("Set manage at class");
		this.manageAtTest=false;
		this.manageAtClass=true;
		return this;
	}
	/**
	 * Do not start/quit a webdriver, if needed, you can control the driver instantiation by calling the `createDriver()`
	 * and `quitDriver(WebDriver driver)` on the SeleniumManager Instance
	 */
	public SeleniumManager setManageNone() {
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
	public SeleniumManager add(IBrowserService browserSvc) {
		log.debug("Add browser service");
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
	public SeleniumManager add(IWatermarkService watermark) {
		log.debug("Add watermark service");
		this.watermark=watermark;
		return this;
	}
	/** 
	 * Attaches a javascript coverage service
	 */
	public SeleniumManager add(IJsCoverageService recorder) {
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

	public boolean usesRemoteDriver() {
		return !JavaCs.isEmpty(currentDriverUrl);
	}
	
	//Driver management
	
	/**
	 * Gets the current driver managed, logs and throws exception is not set
	 * @return
	 */
	public WebDriver driver() {
		if (currentDriver==null) {
			selemaLog.error("The Selenium Manager does not have any active WebDriver");
			throw new SelemaException("The Selenium Manager does not have any active WebDriver");
		}
		return currentDriver; 
	}
    /**
	 * Gets a new WebDriver for the specified class and test. For inernal use only
	 */
	public WebDriver createDriver(String className, String testName) { 
		sessionCount++;
		mediaVideoContext=new MediaContext(conf.getReportDir(), conf.getQualifier(), instanceCount, sessionCount);
		mediaScreenshotContext=new MediaContext(conf.getReportDir(), conf.getQualifier(), instanceCount, sessionCount);
		mediaDiffContext=new MediaContext(conf.getReportDir(), conf.getQualifier(), instanceCount, sessionCount);
		if (!JavaCs.isEmpty(currentDriverUrl)) {
			lastSessionRemote=true;
			selemaLog.info("Remote session " + currentBrowser + " starting...");
			//Colecciona los datos para identificacion de nombre de sesion para visualizacion en selenoid-ui y nombrado de videos
			String driverScope=getDriverScope(className, testName);
			//Colecciona la informacion para localizar posteriormente el instante del fallo en los videos y obtiene el driver
			//El instante antes y despues de creacion del driver sera el margen de tolerancia de este instante
			if (videoRecorder!=null)
				videoRecorder.beforeCreateDriver();
			RemoteWebDriver rdriver=getRemoteSeleniumDriver(driverScope);
			if (videoRecorder!=null)
				videoRecorder.afterCreateDriver(rdriver);
			selemaLog.info("Remote session " + currentBrowser + " started. Remote web driver at " + currentDriverUrl + ", scope: " + driverScope);
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
		currentClassName=className;
		currentTestName="";
		if (manageAtClass)
			createDriver(className, "");
	}
	public void onSetUp(String className, String testName) {
		selemaLog.info("*** SetUp - " + testName);
		currentClassName=className;
		currentTestName=testName;
		if (manageAtTest) {
			createDriver(className, testName);
		}
	}
	public void onTearDown(String className, String testName) {
		selemaLog.info("TearDown - " + testName);
		if (manageAtTest)
			quitDriver(currentDriver, className, testName); 
	}
	public void onTearDownClass(String className, String testName) {
		if (manageAtClass)
			quitDriver(currentDriver, className, testName);
	}
	public String onFailure(String className, String testName) {
		selemaLog.warn("FAIL " + testName);
		String msg="";
		if (currentDriver!=null)
			msg+=screenshotService.takeScreenshot(currentDriver, mediaScreenshotContext, testName);
		if (videoRecorder!=null && lastSessionRemote)
			msg+=videoRecorder.onTestFailure(mediaVideoContext, getDriverScope(className, testName));
		//despues de screenshot y videos por si interfiere en el estado de la pantalla o el instante del fallo
		if (currentDriver!=null && watermark!=null) 
			watermark.fail(currentDriver, testName);
		return msg;
	}
	private String getDriverScope(String className, String testName) {
		String scope = manageAtClass ? className : testName;
		//Le anyade el id del job para evitar mezclar videos cuando se comparte la instancia de selenoid
		scope += " " + ciService.getJobId();
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
		watermark.write(this.driver(), value);
	}
	/**
	 * Asserts if two large strings and links the html differences to the log
	 */
	public void visualAssertEquals(String expected, String actual) {
		visualAssertEquals(expected, actual, "");
	}
	/**
	 * Asserts if two large strings and links the html differences to the log
	 */
	public void visualAssertEquals(String expected, String actual, String message) {
		visualAssertService.assertEquals(expected, actual, message, mediaDiffContext, currentTestName);
	}

	
	private WebDriver getLocalSeleniumDriver() { 
		//Uses a WebDriverManager, setup is only done once per browser
		SeleniumDriverFactory factory=new SeleniumDriverFactory();
		if (!driversWithSetupDone.contains(currentBrowser)) {
			factory.downloadLocalDriver(currentBrowser);
			driversWithSetupDone.add(currentBrowser);
		}
		return factory.getLocalSeleniumDriver(currentBrowser, currentOptions, currentArguments);
	}
	private RemoteWebDriver getRemoteSeleniumDriver(String driverScope) {
		//prepara las opciones anyadiendo a las definidas al configurar, las requeridas por los diferentes servicios
		Map<String,Object> allOptions=new HashMap<>();
		if (currentOptions!=null)
			JavaCs.putAll(allOptions, currentOptions);
		
		//PATCH
		//Although browser service and video recorder are handled independently, in the case of Selenoid:
		//-using Selenium 4.1.0 on .NET, options are not passed to the driver
		//-it is required to pass all selenoid related options as WebDriver protocol extension as a pair "selenoid:options", <map with all options>
		//As currently selenoid is the only supported, temporary makes here the exception
		Map<String,Object> selenoidOptions=new HashMap<>();
		if (browserService!=null)
			JavaCs.putAll(selenoidOptions, browserService.getSeleniumOptions(driverScope));
		if (videoRecorder!=null)
			JavaCs.putAll(selenoidOptions, videoRecorder.getSeleniumOptions(mediaVideoContext, driverScope));
		if (browserService!=null)
			allOptions.put("selenoid:options",selenoidOptions);
		
		return new SeleniumDriverFactory().getRemoteSeleniumDriver(currentBrowser, currentDriverUrl, allOptions, currentArguments);
	}
	
}
