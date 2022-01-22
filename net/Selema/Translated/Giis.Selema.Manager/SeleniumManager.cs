/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Giis.Selema.Portable;
using Giis.Selema.Services;
using Giis.Selema.Services.Impl;
using NLog;
using OpenQA.Selenium;
using Sharpen;

namespace Giis.Selema.Manager
{
	/// <summary>
	/// Core Selema component that manages the appropriate actions in response to the test lifecycle events;
	/// also contains methods for configuration and use during the test,
	/// see https://github.com/javiertuya/selema#readme for instructions
	/// </summary>
	public class SeleniumManager
	{
		internal readonly Logger log = LogManager.GetCurrentClassLogger();

		private ISelemaLogger selemaLog;

		private string currentBrowser = "chrome";

		private string currentDriverUrl = string.Empty;

		private IDictionary<string, object> currentOptions = null;

		private string[] currentArguments = null;

		private IWebDriver currentDriver = null;

		private string currentClassName = string.Empty;

		private string currentTestName = string.Empty;

		//general purpose logger
		//SeleniumManager html log
		//empty is local
		//Currently managed driver
		//Current context, only used for external calls from tests as watermark or getting driver in unmanaged mode
		public virtual string CurrentClassName()
		{
			return currentClassName;
		}

		public virtual string CurrentTestName()
		{
			return currentTestName;
		}

		private bool manageAtTest = true;

		private bool manageAtClass = false;

		private IMediaContext mediaVideoContext;

		private IMediaContext mediaScreenshotContext;

		private IMediaContext mediaDiffContext;

		private static int instanceCount = 0;

		private int sessionCount = 0;

		private bool lastSessionRemote = false;

		private SelemaConfig conf = null;

		private IDriverConfigDelegate driverConfig = null;

		private IWatermarkService watermark = null;

		private ICiService ciService = null;

		private IScreenshotService screenshotService = null;

		private IVisualAssertService visualAssertService = null;

		private IBrowserService browserService = null;

		private IVideoService videoRecorder = null;

		private IJsCoverageService coverageRecorder = null;

		private bool maximizeOnCreate = false;

		/// <summary>Creates an instance with the default configuration</summary>
		public SeleniumManager()
			: this(new SelemaConfig())
		{
		}

		/// <summary>Creates an instance with a given configuration</summary>
		public SeleniumManager(SelemaConfig selemaConfig)
		{
			//Driver modes of operation
			//uniquely identifies each instance of this class
			//uniquely identifies each session (new driver) created by this instance
			//keeps track of the kind of session
			//Behaviour parameters
			if (selemaConfig == null)
			{
				throw new SelemaException("SeleniumManager instance requires an instance of SelemaConfig");
			}
			conf = selemaConfig;
			//ensures report folder is available
			FileUtil.CreateDirectory(conf.GetReportDir());
			this.selemaLog = new LogFactory().GetLogger(conf.GetName(), conf.GetReportDir(), conf.GetLogName());
			//Attach predefined services
			ciService = new CiServiceFactory().GetCurrent();
			screenshotService = new ScreenshotService().Configure(selemaLog);
			visualAssertService = new VisualAssertService().Configure(selemaLog, ciService.IsLocal(), conf.GetProjectRoot(), conf.GetReportSubdir());
			//Other services must be configuresd using add methods
			instanceCount++;
			selemaLog.Info("*** Creating SeleniumManager instance " + instanceCount + " on " + ciService.GetName());
		}

		/// <summary>Gets the current configuration used when instantiating this class</summary>
		public virtual SelemaConfig GetConfig()
		{
			return this.conf;
		}

		/// <summary>Executes the SeleniumManager configuration actions established by the IManagerConfig delegate passed as parameter</summary>
		public virtual Giis.Selema.Manager.SeleniumManager SetManagerDelegate(IManagerConfigDelegate configDelegate)
		{
			configDelegate.Configure(this);
			return this;
		}

		/// <summary>Sets the WebDriver for the specified browser ("chrome","firefox","edge","safari","opera"), default is chrome</summary>
		public virtual Giis.Selema.Manager.SeleniumManager SetBrowser(string browser)
		{
			log.Debug("Set browser: " + browser);
			this.currentBrowser = browser;
			return this;
		}

		/// <summary>Sets a RemoteWebDriver instead a local one (default); The driverUrl must point to the browser service</summary>
		public virtual Giis.Selema.Manager.SeleniumManager SetDriverUrl(string driverUrl)
		{
			log.Debug("Set driver url: " + driverUrl);
			this.currentDriverUrl = driverUrl;
			return this;
		}

		/// <summary>Sets an object that can be used to provide additional configurations to the driver just after its creation</summary>
		public virtual Giis.Selema.Manager.SeleniumManager SetDriverDelegate(IDriverConfigDelegate driverConfig)
		{
			log.Debug("Set driver Config instance");
			this.driverConfig = driverConfig;
			return this;
		}

		public virtual string GetDriverUrl()
		{
			return this.currentDriverUrl;
		}

		/// <summary>Adds the specific capabilities to the WebDriver prior to its creation</summary>
		public virtual Giis.Selema.Manager.SeleniumManager SetOptions(IDictionary<string, object> options)
		{
			log.Debug("Set options: " + options.ToString());
			this.currentOptions = options;
			return this;
		}

		/// <summary>Adds the specific arguments to the WebDriver execution</summary>
		public virtual Giis.Selema.Manager.SeleniumManager SetArguments(string[] arguments)
		{
			log.Debug("Set arguments: " + JavaCs.DeepToString(arguments));
			this.currentArguments = arguments;
			return this;
		}

		/// <summary>Starts the created drivers as maximized</summary>
		public virtual Giis.Selema.Manager.SeleniumManager SetMaximize(bool doMaximize)
		{
			maximizeOnCreate = doMaximize;
			return this;
		}

		/// <summary>Returns to the default behaviour (a driver per each test)</summary>
		public virtual Giis.Selema.Manager.SeleniumManager SetManageAtTest()
		{
			log.Debug("Set manage at test");
			this.manageAtTest = true;
			this.manageAtClass = false;
			return this;
		}

		/// <summary>Starts a WebDriver before the first test at each class, and quits after all tests in the class</summary>
		public virtual Giis.Selema.Manager.SeleniumManager SetManageAtClass()
		{
			log.Debug("Set manage at class");
			this.manageAtTest = false;
			this.manageAtClass = true;
			return this;
		}

		/// <summary>
		/// Do not start/quit a webdriver, if needed, you can control the driver instantiation by calling the `createDriver()`
		/// and `quitDriver(WebDriver driver)` on the SeleniumManager Instance
		/// </summary>
		public virtual Giis.Selema.Manager.SeleniumManager SetManageNone()
		{
			log.Debug("Set unmanaged");
			this.manageAtTest = false;
			this.manageAtClass = false;
			return this;
		}

		public virtual bool GetManageAtTest()
		{
			return this.manageAtTest;
		}

		public virtual bool GetManageAtClass()
		{
			return this.manageAtClass;
		}

		/// <summary>Attaches a browser service (eg selenoid)</summary>
		public virtual Giis.Selema.Manager.SeleniumManager Add(IBrowserService browserSvc)
		{
			log.Debug("Add browser service");
			browserService = browserSvc;
			//when creating this service a compatible video recorder service is created too
			videoRecorder = browserService.GetVideoRecorder();
			if (videoRecorder != null)
			{
				videoRecorder.Configure(selemaLog);
			}
			return this;
		}

		/// <summary>Attaches a watermark service that inserts a text at the top left side of the browser with the name of test being executed and the failure status.</summary>
		public virtual Giis.Selema.Manager.SeleniumManager Add(IWatermarkService watermark)
		{
			log.Debug("Add watermark service");
			this.watermark = watermark;
			return this;
		}

		/// <summary>Attaches a javascript coverage service</summary>
		public virtual Giis.Selema.Manager.SeleniumManager Add(IJsCoverageService recorder)
		{
			log.Debug("Add js coverage service");
			coverageRecorder = recorder.Configure(selemaLog, conf.GetReportDir());
			return this;
		}

		public virtual ISelemaLogger GetLogger()
		{
			return this.selemaLog;
		}

		public virtual ICiService GetCiService()
		{
			return this.ciService;
		}

		public virtual IScreenshotService GetScreenshotService()
		{
			return this.screenshotService;
		}

		public virtual IWatermarkService GetWatermarkService()
		{
			return this.watermark;
		}

		public virtual IJsCoverageService GetCoverageService()
		{
			return this.coverageRecorder;
		}

		public virtual bool UsesRemoteDriver()
		{
			return !JavaCs.IsEmpty(currentDriverUrl);
		}

		//Driver management
		/// <summary>Gets the current driver managed, logs and throws exception is not set</summary>
		public virtual IWebDriver GetDriver()
		{
			if (currentDriver == null)
			{
				selemaLog.Error("The Selenium Manager does not have any active WebDriver");
				throw new SelemaException("The Selenium Manager does not have any active WebDriver");
			}
			return currentDriver;
		}

		/// <summary>
		/// Gets the current driver managed, logs and throws exception is not set
		/// (synonym of getDriver() accessed as a property on net)
		/// </summary>
		public virtual IWebDriver Driver
		{
			get
			{
				return GetDriver();
			}
		}

		/// <summary>Indicates if the web driver has been instantiated</summary>
		public virtual bool HasDriver()
		{
			return currentDriver != null;
		}

		/// <summary>Gets a new WebDriver for the specified class and test.</summary>
		/// <remarks>Gets a new WebDriver for the specified class and test. For inernal use only</remarks>
		public virtual IWebDriver CreateDriver(string className, string testName)
		{
			sessionCount++;
			mediaVideoContext = new MediaContext(conf.GetReportDir(), conf.GetQualifier(), instanceCount, sessionCount);
			mediaScreenshotContext = new MediaContext(conf.GetReportDir(), conf.GetQualifier(), instanceCount, sessionCount);
			mediaDiffContext = new MediaContext(conf.GetReportDir(), conf.GetQualifier(), instanceCount, sessionCount);
			if (this.UsesRemoteDriver())
			{
				lastSessionRemote = true;
				selemaLog.Info("Remote session " + currentBrowser + " starting...");
				//Colecciona los datos para identificacion de nombre de sesion para visualizacion en selenoid-ui y nombrado de videos
				string driverScope = GetDriverScope(className, testName);
				//Colecciona la informacion para localizar posteriormente el instante del fallo en los videos y obtiene el driver
				//El instante antes y despues de creacion del driver sera el margen de tolerancia de este instante
				if (videoRecorder != null)
				{
					videoRecorder.BeforeCreateDriver();
				}
				IWebDriver rdriver = GetRemoteSeleniumDriver(driverScope);
				if (videoRecorder != null)
				{
					videoRecorder.AfterCreateDriver(rdriver);
				}
				selemaLog.Info("Remote session " + currentBrowser + " started. Remote web driver at " + currentDriverUrl + ", scope: " + driverScope);
				currentDriver = rdriver;
			}
			else
			{
				lastSessionRemote = false;
				string args = string.Empty;
				selemaLog.Info("Local session " + currentBrowser + " starting" + args + " ...");
				currentDriver = GetLocalSeleniumDriver();
			}
			if (coverageRecorder != null)
			{
				coverageRecorder.AfterCreateDriver(currentDriver);
			}
			if (maximizeOnCreate)
			{
				currentDriver.Manage().Window.Maximize();
			}
			if (driverConfig != null)
			{
				driverConfig.Configure(currentDriver);
			}
			return currentDriver;
		}

		/// <summary>Gets a new WebDriver for the current class and test, use when running in unmanaged mode</summary>
		public virtual IWebDriver CreateDriver()
		{
			return CreateDriver(currentClassName, currentTestName);
		}

		/// <summary>Quits the WebDriver for the specified class and test.</summary>
		/// <remarks>Quits the WebDriver for the specified class and test. For inernal use only</remarks>
		public virtual IWebDriver QuitDriver(IWebDriver driver, string className, string testName)
		{
			// Methods returns should not be invariant: here the goal is to reset the driver variable
			if (driver == null)
			{
				return null;
			}
			if (coverageRecorder != null)
			{
				coverageRecorder.BeforeQuitDriver(driver);
			}
			if (videoRecorder != null && lastSessionRemote)
			{
				videoRecorder.BeforeQuitDriver(mediaVideoContext, GetDriverScope(className, testName));
				selemaLog.Info("Remote session ending");
			}
			driver.Quit();
			currentDriver = null;
			return currentDriver;
		}

		//
		/// <summary>Quits the WebDriver for the current class and test, use when running in unmanaged mode</summary>
		public virtual IWebDriver QuitDriver(IWebDriver driver)
		{
			return QuitDriver(driver, currentClassName, currentTestName);
		}

		/// <summary>Replaces the WebDriver for the specified class and test by the specified.</summary>
		/// <remarks>Replaces the WebDriver for the specified class and test by the specified. For inernal use only</remarks>
		public virtual void ReplaceDriver(IWebDriver driver)
		{
			currentDriver = driver;
		}

		//Response to lifecycle events
		public virtual void OnSetUpClass(string className)
		{
			log.Trace("on set up class " + className);
			currentClassName = className;
			currentTestName = string.Empty;
			if (manageAtClass)
			{
				CreateDriver(className, string.Empty);
			}
		}

		public virtual void OnSetUp(string className, string testName)
		{
			log.Trace("on set up test " + testName);
			selemaLog.Info("*** SetUp - " + testName);
			currentClassName = className;
			currentTestName = testName;
			if (manageAtTest)
			{
				CreateDriver(className, testName);
			}
		}

		public virtual void OnTearDown(string className, string testName)
		{
			log.Trace("on tear down test " + testName);
			selemaLog.Info("TearDown - " + testName);
			if (manageAtTest)
			{
				QuitDriver(currentDriver, className, testName);
			}
		}

		public virtual void OnTearDownClass(string className, string testName)
		{
			log.Trace("on tear down class " + className);
			if (manageAtClass)
			{
				QuitDriver(currentDriver, className, testName);
			}
		}

		public virtual string OnFailure(string className, string testName)
		{
			log.Trace("on test failure " + testName);
			selemaLog.Warn("FAIL " + testName);
			string msg = string.Empty;
			if (currentDriver != null)
			{
				msg += screenshotService.TakeScreenshot(currentDriver, mediaScreenshotContext, testName);
			}
			if (videoRecorder != null && lastSessionRemote)
			{
				msg += videoRecorder.OnTestFailure(mediaVideoContext, GetDriverScope(className, testName));
			}
			//despues de screenshot y videos por si interfiere en el estado de la pantalla o el instante del fallo
			if (currentDriver != null && watermark != null)
			{
				watermark.Fail(currentDriver, testName);
			}
			return msg;
		}

		public virtual void OnSuccess(string testName)
		{
			log.Trace("on test success " + testName);
			selemaLog.Info("SUCCESS " + testName);
		}

		private string GetDriverScope(string className, string testName)
		{
			string scope = manageAtClass ? className : testName;
			//Le anyade el id del job para evitar mezclar videos cuando se comparte la instancia de selenoid
			scope += " " + this.GetCiService().GetJobId();
			return scope;
		}

		//Proxies to some common services
		/// <summary>Takes a screenshot to a file that will be linked to the log</summary>
		public virtual void Screenshot(string fileName)
		{
			screenshotService.TakeScreenshot(this.Driver, mediaScreenshotContext, fileName);
		}

		/// <summary>Places a watermark with the test name (requires the watermark service be attached)</summary>
		public virtual void Watermark()
		{
			WatermarkText(currentTestName);
		}

		/// <summary>Places a watermark with a given text (requires the watermark service be attached)</summary>
		public virtual void WatermarkText(string value)
		{
			if (watermark == null)
			{
				selemaLog.Error("Watermark service is not attached to this Selenium Manager");
				throw new SelemaException("Watermark service is not attached to this Selenium Manager");
			}
			watermark.Write(this.Driver, value);
		}

		/// <summary>Asserts if two large strings and links the html differences to the log</summary>
		public virtual void VisualAssertEquals(string expected, string actual)
		{
			VisualAssertEquals(expected, actual, string.Empty);
		}

		/// <summary>Asserts if two large strings and links the html differences to the log</summary>
		public virtual void VisualAssertEquals(string expected, string actual, string message)
		{
			visualAssertService.AssertEquals(expected, actual, message, mediaDiffContext, currentTestName);
		}

		private IWebDriver GetLocalSeleniumDriver()
		{
			log.Trace("Get local Selenium Driver");
			return new SeleniumDriverFactory().GetSeleniumDriver(currentBrowser, string.Empty, currentOptions, currentArguments);
		}

		private IWebDriver GetRemoteSeleniumDriver(string driverScope)
		{
			log.Trace("Get remote Selenium Driver");
			//prepara las opciones anyadiendo a las definidas al configurar, las requeridas por los diferentes servicios
			IDictionary<string, object> allOptions = new Dictionary<string, object>();
			if (currentOptions != null)
			{
				JavaCs.PutAll(allOptions, currentOptions);
			}
			//PATCH
			//Although browser service and video recorder are handled independently, in the case of Selenoid:
			//-using Selenium 4.1.0 on .NET, options are not passed to the driver
			//-it is required to pass all selenoid related options as WebDriver protocol extension as a pair "selenoid:options", <map with all options>
			//As currently selenoid is the only supported, temporary makes here the exception
			IDictionary<string, object> selenoidOptions = new Dictionary<string, object>();
			if (browserService != null)
			{
				JavaCs.PutAll(selenoidOptions, browserService.GetSeleniumOptions(driverScope));
			}
			if (videoRecorder != null)
			{
				JavaCs.PutAll(selenoidOptions, videoRecorder.GetSeleniumOptions(mediaVideoContext, driverScope));
			}
			if (browserService != null)
			{
				allOptions["selenoid:options"] = selenoidOptions;
			}
			return new SeleniumDriverFactory().GetSeleniumDriver(currentBrowser, currentDriverUrl, allOptions, currentArguments);
		}
	}
}
