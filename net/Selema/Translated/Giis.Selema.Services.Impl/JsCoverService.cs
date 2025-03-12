using OpenQA.Selenium;
using NLog;
using Giis.Portable.Util;
using Giis.Selema.Portable.Selenium;
using Giis.Selema.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services.Impl
{
    /// <summary>
    /// Support for JSCover javascript coverage.
    /// </summary>
    public class JsCoverService : IJsCoverageService
    {
        readonly Logger log = LogManager.GetCurrentClassLogger();
        private ISelemaLogger selemaLog;
        //html to reset browser local memory
        private static readonly string CLEAR_LOCAL_STORAGE_HTML = "/jscoverage-clear-local-storage.html";
        //html to restore browser local memory from file
        private static readonly string RESTORE_LOCAL_STORAGE_HTML = "/jscoverage-restore-local-storage.html";
        //js script executed to save coverage from browser local memory to file
        private static readonly string SAVE_COVERAGE_SCRIPT = "return jscoverage_serializeCoverageToJSON();";
        //js script to restore coverage from file
        private static readonly string RESTORE_COVERAGE_SCRIPT = "restoreLocalStorage()";
        private string appRoot;
        private string reportDir;
        private string savedCoverageFile;
        private bool resetDone = false;
        //This shall be instantiated as a singleton
        private static JsCoverService instance;
        private JsCoverService(string appRootUrl)
        {
            this.appRoot = appRootUrl;
            this.savedCoverageFile = "jscoverage.json";
        }

        /// <summary>
        /// Instantiation of this service as a singleton
        /// </summary>
        public static JsCoverService GetInstance(string appRootUrl)
        {
            if (instance == null)
                instance = new JsCoverService(appRootUrl);
            return instance;
        }

        /// <summary>
        /// Configures this service, called on attaching the service to a SeleManager
        /// </summary>
        public virtual IJsCoverageService Configure(ISelemaLogger thisLog, string reportDir)
        {
            this.selemaLog = thisLog;
            this.reportDir = reportDir;
            FileUtil.CreateDirectory(reportDir); //ensures the report folder exists
            return this;
        }

        /// <summary>
        /// Resets coverage stored in browser local memory and restores coverage from previous sessions.
        /// Reset is made calling an html file created by the JSCover instrumentation, executed only the first time.
        /// Subsequent executions call an additional html file to restore coverage from previous executions.
        /// </summary>
        public virtual void AfterCreateDriver(IWebDriver driver)
        {
            string coverageFileName = FileUtil.GetPath(this.reportDir, savedCoverageFile);
            if (!resetDone)
            {
                DriverUtil.GetUrl(driver, appRoot + CLEAR_LOCAL_STORAGE_HTML);

                //Creates a first default file without coverage data
                //(avoids reporting failures if execution does not produces any coverage data)
                string defaultCoverage = "{\"/EMPTY-JS-COVERAGE.js\":{\"lineData\":[null,0,0],\"functionData\":[0],\"branchData\":{}}}";
                FileUtil.FileWrite(coverageFileName, defaultCoverage);
            }
            else
            {
                DriverUtil.GetUrl(driver, appRoot + RESTORE_LOCAL_STORAGE_HTML);

                //reads from file the previous coverage data and sends to browser local memory
                try
                {
                    string coverage = FileUtil.FileRead(coverageFileName);
                    SeleniumActions.SetInnerText(driver, "coverageToRestore", coverage);
                    SeleniumActions.ExecuteScript(driver, RESTORE_COVERAGE_SCRIPT);
                }
                catch (Exception e)
                {
                    selemaLog.Error("Exception reading js coverage file: " + e.Message);
                }
            }

            resetDone = true;
        }

        /// <summary>
        /// Saves coverage from browser local memory to the json external file jscoverage.json
        /// </summary>
        public virtual void BeforeQuitDriver(IWebDriver driver)
        {
            string coverageFileName = FileUtil.GetPath(reportDir, savedCoverageFile);
            try
            {
                selemaLog.Debug("Saving JSCover coverage from browser local storage to file " + coverageFileName);
                string jsonCoverage = SeleniumActions.ExecuteScript(driver, SAVE_COVERAGE_SCRIPT);
                selemaLog.Trace("Content: " + jsonCoverage);
                FileUtil.FileWrite(coverageFileName, jsonCoverage);
            }
            catch (Exception e)
            {
                selemaLog.Error("Can't get JSCover coverage, either code is not instrumented or output file " + coverageFileName + " can't be saved: " + e.Message);
            }
        }

        /// <summary>
        /// Sets the file name where coverage is saved, only for testing
        /// </summary>
        public virtual void SetCoverageFileName(string name)
        {
            this.savedCoverageFile = name;
        }
    }
}