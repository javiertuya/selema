using Java.Util;
using NLog;
using OpenQA.Selenium;
using Giis.Portable.Util;
using Giis.Selema.Services;
using Giis.Selema.Services.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Manager
{
    /// <summary>
    /// Core Selema component that manages the appropriate actions in response to the test lifecycle events;
    /// also contains methods for configuration and use during the test,
    /// see https://github.com/javiertuya/selema#readme for instructions
    /// </summary>
    public class SeleManager
    {
        readonly Logger log = LogManager.GetCurrentClassLogger(); //general purpose logger
        private ISelemaLogger selemaLog; //SeleManager html log
        private string currentBrowser = "chrome";
        private string currentDriverUrl = ""; //empty is local
        private Map<string, object> currentOptions = null;
        private string[] currentArguments = null;
        private DriverOptions currentOptionsInstance = null;
        private string driverVersion = DriverVersion.DEFAULT; // string with strategy to use or a given version
        // Control of retries during remote driver warm-up
        long startTime = JavaCs.CurrentTimeMillis();
        int warmUpPeriod = 0; // default 0: no retry
        //Currently managed driver
        private IWebDriver currentDriver = null;
        //Current context, only used for external calls from tests as watermark or getting driver in unmanaged mode
        private string currentClassName = "";
        private string currentTestName = "";
        public virtual string CurrentClassName()
        {
            return currentClassName;
        }

        public virtual string CurrentTestName()
        {
            return currentTestName;
        }

        //Driver modes of operation
        private bool manageAtTest = true;
        private bool manageAtClass = false;
        private IMediaContext mediaVideoContext;
        private IMediaContext mediaScreenshotContext;
        private IMediaContext mediaDiffContext;
        private static int instanceCount = 0; //uniquely identifies each instance of this class
        private int sessionCount = 0; //uniquely identifies each session (new driver) created by this instance
        private bool lastSessionRemote = false; //keeps track of the kind of session
        //Behaviour parameters
        private SelemaConfig conf = null;
        private IDriverConfigDelegate driverConfig = null;
        private IWatermarkService watermark = null;
        private ICiService ciService = null;
        private IScreenshotService screenshotService = null;
        private IVisualAssertService visualAssertService = null;
        private ISoftAssertService softAssertService = null;
        private IBrowserService browserService = null;
        private IVideoService videoRecorder = null;
        private IJsCoverageService coverageRecorder = null;
        private bool maximizeOnCreate = false;
        /// <summary>
        /// Creates an instance with the default configuration
        /// </summary>
        public SeleManager() : this(new SelemaConfig())
        {
        }

        /// <summary>
        /// Creates an instance with a given configuration
        /// </summary>
        public SeleManager(SelemaConfig selemaConfig)
        {
            if (selemaConfig == null)
                throw new SelemaException("SeleManager instance requires an instance of SelemaConfig");
            conf = selemaConfig;

            //ensures report folder is available
            FileUtil.CreateDirectory(conf.GetReportDir());
            this.selemaLog = new LogFactory().GetLogger(conf.GetName(), conf.GetReportDir(), conf.GetLogName());

            //Attach predefined services
            ciService = new CiServiceFactory().GetCurrent();
            screenshotService = new ScreenshotService().Configure(selemaLog);
            visualAssertService = new VisualAssertService().Configure(selemaLog, ciService.IsLocal(), conf.GetProjectRoot(), conf.GetReportSubdir());
            softAssertService = new SoftAssertService().Configure(selemaLog, ciService.IsLocal(), conf.GetProjectRoot(), conf.GetReportSubdir());

            //Other services must be configuresd using add methods
            instanceCount++;
            selemaLog.Info("*** Creating SeleManager instance " + instanceCount + " on " + ciService.GetName());
        }

        /// <summary>
        /// Gets the current configuration used when instantiating this class
        /// </summary>
        public virtual SelemaConfig GetConfig()
        {
            return this.conf;
        }

        /// <summary>
        /// Executes the SeleManager configuration actions established by the IManagerConfig delegate passed as parameter
        /// </summary>
        public virtual SeleManager SetManagerDelegate(IManagerConfigDelegate configDelegate)
        {
            configDelegate.Configure(this);
            return this;
        }

        /// <summary>
        /// Configures the IWebDriver for the specified browser ("chrome","firefox","edge","safari","opera"), default is chrome
        /// </summary>
        public virtual SeleManager SetBrowser(string browser)
        {
            log.Debug("Set browser: " + browser);
            this.currentBrowser = browser;
            return this;
        }

        /// <summary>
        /// Configures a RemoteWebDriver instead of the default local one; The driverUrl must point to the browser service
        /// </summary>
        public virtual SeleManager SetDriverUrl(string driverUrl)
        {
            log.Debug("Set driver url: " + driverUrl);
            this.currentDriverUrl = driverUrl;
            return this;
        }

        /// <summary>
        /// Configures a RemoteWebDriver instead of the default local one, with an optional warm-up period specified in seconds.
        /// Setting this value to a positive number helps in scenarios where a test initializes the driver immediately after
        /// the remote browser server starts, which may require a few seconds to become ready. During this warm-up period
        /// (measured from the manager's instantiation) any failures in obtaining the driver will be retried.
        /// </summary>
        public virtual SeleManager SetDriverUrl(string driverUrl, int warmUpPeriod)
        {
            this.warmUpPeriod = warmUpPeriod;
            return SetDriverUrl(driverUrl);
        }

        /// <summary>
        /// Configures an object that can be used to provide additional configurations to the driver just after its creation
        /// </summary>
        public virtual SeleManager SetDriverDelegate(IDriverConfigDelegate driverConfig)
        {
            log.Debug("Set driver Config instance");
            this.driverConfig = driverConfig;
            return this;
        }

        public virtual string GetDriverUrl()
        {
            return this.currentDriverUrl;
        }

        /// <summary>
        /// Configures the driver version selection strategy or the value of the desired driver version to set
        /// </summary>
        public virtual SeleManager SetDriverVersion(string driverVersion)
        {
            this.driverVersion = driverVersion;
            return this;
        }

        /// <summary>
        /// Adds the specific capabilities to the IWebDriver prior to its creation
        /// </summary>
        public virtual SeleManager SetOptions(Map<string, object> options)
        {
            log.Debug("Set options: " + options.ToString());
            this.currentOptions = options;
            return this;
        }

        /// <summary>
        /// Adds a browser dependent instance of options to set the W3C IWebDriver standard capabilities.
        /// The capabilities specified with setOptions and setArguments
        /// will be added as well to the IWebDriver prior to its creation
        /// </summary>
        public virtual SeleManager SetOptionsInstance(DriverOptions optionsInstance)
        {
            log.Debug("Set options instance: " + optionsInstance.GetType().FullName);
            this.currentOptionsInstance = optionsInstance;
            return this;
        }

        /// <summary>
        /// Adds the specific arguments to the IWebDriver execution
        /// </summary>
        public virtual SeleManager SetArguments(string[] arguments)
        {
            log.Debug("Set arguments: " + JavaCs.DeepToString(arguments));
            this.currentArguments = arguments;
            return this;
        }

        /// <summary>
        /// Starts the created drivers as maximized
        /// </summary>
        public virtual SeleManager SetMaximize(bool doMaximize)
        {
            maximizeOnCreate = doMaximize;
            return this;
        }

        /// <summary>
        /// Returns to the default behaviour (a driver per each test)
        /// </summary>
        public virtual SeleManager SetManageAtTest()
        {
            log.Debug("Set manage at test");
            this.manageAtTest = true;
            this.manageAtClass = false;
            return this;
        }

        /// <summary>
        /// Starts a IWebDriver before the first test at each class, and quits after all tests in the class
        /// </summary>
        public virtual SeleManager SetManageAtClass()
        {
            log.Debug("Set manage at class");
            this.manageAtTest = false;
            this.manageAtClass = true;
            return this;
        }

        /// <summary>
        /// Do not start/quit a webdriver, if needed, you can control the driver instantiation by calling the `createDriver()`
        /// and `quitDriver(IWebDriver driver)` on the SeleManager Instance
        /// </summary>
        public virtual SeleManager SetManageNone()
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

        /// <summary>
        /// Attaches a browser service (eg selenoid)
        /// </summary>
        public virtual SeleManager Add(IBrowserService browserSvc)
        {
            log.Debug("Add browser service: " + browserSvc.GetType().Name);
            browserService = browserSvc;

            //when creating this service a compatible video recorder service is created too
            videoRecorder = browserService.GetNewVideoRecorder();
            if (videoRecorder != null)
                videoRecorder.Configure(selemaLog);
            return this;
        }

        /// <summary>
        /// Attaches a watermark service that inserts a text at the top left side of the browser with the name of test being executed and the failure status.
        /// </summary>
        public virtual SeleManager Add(IWatermarkService watermark)
        {
            log.Debug("Add watermark service");
            this.watermark = watermark;
            return this;
        }

        /// <summary>
        /// Attaches a javascript coverage service
        /// </summary>
        public virtual SeleManager Add(IJsCoverageService recorder)
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
        /// <summary>
        /// Gets the current driver managed, logs and throws exception is not set
        /// </summary>
        public virtual IWebDriver GetDriver()
        {
            if (currentDriver == null)
            {
                selemaLog.Error("The Selenium Manager does not have any active IWebDriver");
                throw new SelemaException("The Selenium Manager does not have any active IWebDriver");
            }

            return currentDriver;
        }

        /// <summary>
        /// Gets the current driver managed, logs and throws exception is not set
        /// (synonym of getDriver() accessed as a property on net)
        /// </summary>
        public IWebDriver Driver { get { return this.GetDriver(); } }  public virtual IWebDriver DriverJavaOnly()
        {
            return GetDriver();
        }

        /// <summary>
        /// Indicates if the web driver has been instantiated
        /// </summary>
        public virtual bool HasDriver()
        {
            return currentDriver != null;
        }

        /// <summary>
        /// Gets a new IWebDriver for the specified class and test. For inernal use only
        /// </summary>
        public virtual IWebDriver CreateDriver(string className, string testName)
        {
            sessionCount++;
            mediaVideoContext = new MediaContext(conf.GetReportDir(), conf.GetQualifier(), instanceCount, sessionCount);
            mediaScreenshotContext = new MediaContext(conf.GetReportDir(), conf.GetQualifier(), instanceCount, sessionCount);
            mediaDiffContext = new MediaContext(conf.GetReportDir(), conf.GetQualifier(), instanceCount, sessionCount);
            if (this.UsesRemoteDriver())
            {
                lastSessionRemote = true;
                selemaLog.Info("Remote session " + currentBrowser + " starting on " + currentDriverUrl + " ...");

                //Colecciona los datos para identificacion de nombre de sesion para visualizacion en selenoid-ui y nombrado de videos
                string driverScope = GetDriverScope(className, testName);

                //Colecciona la informacion para localizar posteriormente el instante del fallo en los videos y obtiene el driver
                //El instante antes y despues de creacion del driver sera el margen de tolerancia de este instante
                if (videoRecorder != null)
                    videoRecorder.BeforeCreateDriver();
                long timestamp = JavaCs.CurrentTimeMillis();
                IWebDriver rdriver = GetRemoteSeleniumDriver(driverScope);
                selemaLog.Info("Remote session " + currentBrowser + " started. Scope: " + driverScope + ". Time: " + (JavaCs.CurrentTimeMillis() - timestamp) + "ms");
                if (videoRecorder != null)
                    videoRecorder.AfterCreateDriver(rdriver);
                currentDriver = rdriver;
            }
            else
            {
                lastSessionRemote = false;
                string args = "";
                selemaLog.Info("Local session " + currentBrowser + " starting" + args + " ...");
                currentDriver = GetLocalSeleniumDriver();
            }

            if (coverageRecorder != null)
                coverageRecorder.AfterCreateDriver(currentDriver);
            if (maximizeOnCreate)
                currentDriver.Manage().Window.Maximize();
            if (driverConfig != null)
                driverConfig.Configure(currentDriver);
            return currentDriver;
        }

        /// <summary>
        /// Gets a new IWebDriver for the current class and test, use when running in unmanaged mode
        /// </summary>
        public virtual IWebDriver CreateDriver()
        {
            return CreateDriver(currentClassName, currentTestName);
        }

        /// <summary>
        /// Quits the IWebDriver for the specified class and test. For inernal use only
        /// </summary>
        public virtual IWebDriver QuitDriver(IWebDriver driver, string className, string testName)
        {

            //NOSONAR Methods returns should not be invariant: here the goal is to reset the driver variable
            if (driver == null)
                return null;
            if (coverageRecorder != null)
                coverageRecorder.BeforeQuitDriver(driver);
            if (videoRecorder != null && lastSessionRemote)
            {
                videoRecorder.BeforeQuitDriver(mediaVideoContext, GetDriverScope(className, testName));
                selemaLog.Info("Remote session ending");
            }

            driver.Quit();
            currentDriver = null;
            return currentDriver; //NOSONAR
        }

        /// <summary>
        /// Quits the IWebDriver for the current class and test, use when running in unmanaged mode
        /// </summary>
        public virtual IWebDriver QuitDriver(IWebDriver driver)
        {
            return QuitDriver(driver, currentClassName, currentTestName);
        }

        /// <summary>
        /// Replaces the IWebDriver for the specified class and test by the specified. For inernal use only
        /// </summary>
        public virtual void ReplaceDriver(IWebDriver driver)
        {
            currentDriver = driver;
        }

        //Response to lifecycle events
        public virtual void OnSetUpClass(string className)
        {
            log.Trace("on set up class " + className);
            currentClassName = className;
            currentTestName = "";
            if (manageAtClass)
                CreateDriver(className, "");
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
                QuitDriver(currentDriver, className, testName);
        }

        public virtual void OnTearDownClass(string className, string testName)
        {
            log.Trace("on tear down class " + className);
            if (manageAtClass)
                QuitDriver(currentDriver, className, testName);
        }

        public virtual string OnFailure(string className, string testName)
        {
            log.Trace("on test failure " + testName);
            selemaLog.Warn("FAIL " + testName);
            string msg = "";
            if (currentDriver != null)
                msg += screenshotService.TakeScreenshot(currentDriver, mediaScreenshotContext, testName);
            if (videoRecorder != null && lastSessionRemote)
                msg += videoRecorder.OnTestFailure(mediaVideoContext, GetDriverScope(className, testName));

            //despues de screenshot y videos por si interfiere en el estado de la pantalla o el instante del fallo
            if (currentDriver != null && watermark != null)
                msg += WatermarkFail(currentDriver, testName);
            return msg;
        }

        // After update to Selenium 4.11 we detected some tests that failed with "invalid session id" message.
        // This was caused by button cliks using javascript executor that apparently crashed the browser
        // and cause a fist failure in the watermark writing that is controlled and logged now
        private string WatermarkFail(IWebDriver driver, string value)
        {
            try
            {
                watermark.Fail(driver, value);
                return "";
            }
            catch (Exception e)
            {
                string msg = "Can't write onFailure watermark " + value + ". Message: " + e.Message;
                Giis.Portable.Util.NLogUtil.Error(log, msg);
                selemaLog.Error(msg);
                return msg;
            }
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
        /// <summary>
        /// Takes a screenshot to a file that will be linked to the log
        /// </summary>
        public virtual void Screenshot(string fileName)
        {
            screenshotService.TakeScreenshot(this.Driver, mediaScreenshotContext, fileName);
        }

        /// <summary>
        /// Places a watermark with the test name (requires the watermark service be attached)
        /// </summary>
        public virtual void Watermark()
        {
            WatermarkText(currentTestName);
        }

        /// <summary>
        /// Places a watermark with a given text (requires the watermark service be attached)
        /// </summary>
        public virtual void WatermarkText(string value)
        {
            if (watermark == null)
            {
                selemaLog.Error("Watermark service is not attached to this Selenium Manager");
                throw new SelemaException("Watermark service is not attached to this Selenium Manager");
            }

            try
            {
                watermark.Write(this.Driver, value);
            }
            catch (Exception e)
            {
                string msg = "Can't write onFailure watermark " + value + ". Message: " + e.Message;
                Giis.Portable.Util.NLogUtil.Warn(log, msg);
                selemaLog.Warn(msg);
            }
        }

        /// <summary>
        /// Asserts if two large strings and links the html differences to the log
        /// </summary>
        public virtual void VisualAssertEquals(string expected, string actual)
        {
            VisualAssertEquals(expected, actual, "");
        }

        /// <summary>
        /// Asserts if two large strings and links the html differences to the log, including an additional message
        /// </summary>
        public virtual void VisualAssertEquals(string expected, string actual, string message)
        {
            visualAssertService.AssertEquals(expected, actual, message, mediaDiffContext, currentTestName);
        }

        /// <summary>
        /// Soft Asserts if two large strings and links the html differences to the log:
        /// records the assertion message instead of throwing and exception until softAssertAll() is called
        /// </summary>
        public virtual void SoftAssertEquals(string expected, string actual)
        {
            SoftAssertEquals(expected, actual, "");
        }

        /// <summary>
        /// Soft Asserts if two large strings and links the html differences to the log with an additonal message:
        /// records the assertion message instead of throwing and exception until softAssertAll() is called
        /// </summary>
        public virtual void SoftAssertEquals(string expected, string actual, string message)
        {
            softAssertService.AssertEquals(expected, actual, message, mediaDiffContext, currentTestName);
        }

        /// <summary>
        /// Throws and exception if at least one soft assertion failed including all assertion messages
        /// </summary>
        public virtual void SoftAssertAll()
        {
            softAssertService.AssertAll();
        }

        /// <summary>
        /// Resets the current soft assertion failure messages that are stored
        /// </summary>
        public virtual void SoftAssertClear()
        {
            softAssertService.AssertClear();
        }

        private IWebDriver GetLocalSeleniumDriver()
        {
            log.Trace("Get local Selenium Driver");
            return new SeleniumDriverFactory().GetSeleniumDriver(currentBrowser, "", this.driverVersion, currentOptions, currentArguments, currentOptionsInstance);
        }

        private IWebDriver GetRemoteSeleniumDriver(string driverScope)
        {
            log.Trace("Get remote Selenium Driver");

            //prepara las opciones anyadiendo a las definidas al configurar, las requeridas por los diferentes servicios
            Map<string, object> allOptions = new HashMap<string, object>(); // NOSONAR net compatibility
            if (currentOptions != null)
                allOptions.PutAll(currentOptions);
            if (browserService != null)
                browserService.AddBrowserServiceOptions(allOptions, videoRecorder, mediaVideoContext, driverScope);

            // Retry during warm-up period (if set to a positive number of seconds).
            // This helps to avoid unrecoverable exceptions if the browser server was started just before creating the
            // driver and it is not already ready.
            int retryCount = 0;
            while (warmUpPeriod > 0 && JavaCs.CurrentTimeMillis() < startTime + warmUpPeriod * 1000)
            {
                try
                {
                    return new SeleniumDriverFactory().GetSeleniumDriver(currentBrowser, currentDriverUrl, driverVersion, allOptions, currentArguments, currentOptionsInstance);
                }
                catch (Exception e)
                {
                    string message = "Remote driver creation failure during warm-up period of " + warmUpPeriod + " seconds. Retry " + retryCount++;
                    this.selemaLog.Warn(message);
                    JavaCs.Sleep(1000);
                }
            }


            // unconditional creation of driver (if warm-up period not set or fails after the time elapsed)
            return new SeleniumDriverFactory().GetSeleniumDriver(currentBrowser, currentDriverUrl, driverVersion, allOptions, currentArguments, currentOptionsInstance);
        }
    }
}