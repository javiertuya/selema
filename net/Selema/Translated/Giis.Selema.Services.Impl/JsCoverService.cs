/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Giis.Selema.Portable;
using Giis.Selema.Portable.Selenium;
using Giis.Selema.Services;
using NLog;
using OpenQA.Selenium;
using Sharpen;

namespace Giis.Selema.Services.Impl
{
	/// <summary>Support for JSCover javascript coverage.</summary>
	public class JsCoverService : IJsCoverageService
	{
		internal readonly Logger log = LogManager.GetCurrentClassLogger();

		private ISelemaLogger selemaLog;

		private const string ClearLocalStorageHtml = "/jscoverage-clear-local-storage.html";

		private const string RestoreLocalStorageHtml = "/jscoverage-restore-local-storage.html";

		private const string SaveCoverageScript = "return jscoverage_serializeCoverageToJSON();";

		private const string RestoreCoverageScript = "restoreLocalStorage()";

		private string appRoot;

		private string reportDir;

		private string savedCoverageFile;

		private bool resetDone = false;

		private static Giis.Selema.Services.Impl.JsCoverService instance;

		private JsCoverService(string appRootUrl)
		{
			//html to reset browser local memory
			//html to restore browser local memory from file
			//js script executed to save coverage from browser local memory to file
			//js script to restore coverage from file
			//This shall be instantiated as a singleton
			this.appRoot = appRootUrl;
			this.savedCoverageFile = "jscoverage.json";
		}

		/// <summary>Instantiation of this service as a singleton</summary>
		public static Giis.Selema.Services.Impl.JsCoverService GetInstance(string appRootUrl)
		{
			if (instance == null)
			{
				instance = new Giis.Selema.Services.Impl.JsCoverService(appRootUrl);
			}
			return instance;
		}

		/// <summary>Configures this service, called on attaching the service to a SeleniumManager</summary>
		public virtual IJsCoverageService Configure(ISelemaLogger thisLog, string reportDir)
		{
			this.selemaLog = thisLog;
			this.reportDir = reportDir;
			FileUtil.CreateDirectory(reportDir);
			//ensures the report folder exists
			return this;
		}

		/// <summary>Resets coverage stored in browser local memory and restores coverage from previous sessions.</summary>
		/// <remarks>
		/// Resets coverage stored in browser local memory and restores coverage from previous sessions.
		/// Reset is made calling an html file created by the JSCover instrumentation, executed only the first time.
		/// Subsequent executions call an additional html file to restore coverage from previous executions.
		/// </remarks>
		public virtual void AfterCreateDriver(IWebDriver driver)
		{
			string coverageFileName = FileUtil.GetPath(this.reportDir, savedCoverageFile);
			if (!resetDone)
			{
				driver.Url = appRoot + ClearLocalStorageHtml;
				//Creates a first default file without coverage data
				//(avoids reporting failures if execution does not produces any coverage data)
				string defaultCoverage = "{\"/EMPTY-JS-COVERAGE.js\":{\"lineData\":[null,0,0],\"functionData\":[0],\"branchData\":{}}}";
				FileUtil.FileWrite(coverageFileName, defaultCoverage);
			}
			else
			{
				driver.Url = appRoot + RestoreLocalStorageHtml;
				//reads from file the previous coverage data and sends to browser local memory
				try
				{
					string coverage = FileUtil.FileRead(coverageFileName);
					SeleniumActions.SetInnerText(driver, "coverageToRestore", coverage);
					SeleniumActions.ExecuteScript(driver, RestoreCoverageScript);
				}
				catch (Exception e)
				{
					selemaLog.Error("Exception reading js coverage file: " + e.Message);
				}
			}
			resetDone = true;
		}

		/// <summary>Saves coverage from browser local memory to the json external file jscoverage.json</summary>
		public virtual void BeforeQuitDriver(IWebDriver driver)
		{
			string coverageFileName = FileUtil.GetPath(reportDir, savedCoverageFile);
			try
			{
				selemaLog.Debug("Saving JSCover coverage from browser local storage to file " + coverageFileName);
				string jsonCoverage = SeleniumActions.ExecuteScript(driver, SaveCoverageScript);
				selemaLog.Trace("Content: " + jsonCoverage);
				FileUtil.FileWrite(coverageFileName, jsonCoverage);
			}
			catch (Exception e)
			{
				selemaLog.Error("Can't get JSCover coverage, either code is not instrumented or output file " + coverageFileName + " can't be saved: " + e.Message);
			}
		}

		/// <summary>Sets the file name where coverage is saved, only for testing</summary>
		public virtual void SetCoverageFileName(string name)
		{
			this.savedCoverageFile = name;
		}
	}
}
