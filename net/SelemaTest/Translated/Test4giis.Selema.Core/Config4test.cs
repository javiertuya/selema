using Java.Util;
using Giis.Portable.Util;
using Giis.Selema.Manager;
using Giis.Selema.Portable;
using Giis.Selema.Services;
using Giis.Selema.Services.Browser;
using Giis.Selema.Services.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Core
{
    public class Config4test : IManagerConfigDelegate
    {
        private string PROPERTIES_FILE = FileUtil.GetPath(new SelemaConfig().GetProjectRoot(), "selema.properties");
        private Properties prop;
        private bool useWatermark;
        public Config4test() : this(true)
        {
        }

        public Config4test(bool useWatermark)
        {
            this.useWatermark = useWatermark;
            prop = new PropertiesFactory().GetPropertiesFromFilename(PROPERTIES_FILE);
            if (prop == null)
                throw new Exception("Can't load test configuration: selema.properties");
        }

        //default selema config for tests
        public static SelemaConfig GetConfig()
        {

            //by default test reports located in subfolder
            return new SelemaConfig().SetReportSubdir(FileUtil.GetPath(Parameters.DefaultReportSubdir + "/selema"));
        }

        //implements the interface IManagerConfig to establish the default configuration for test
        public virtual void Configure(SeleManager sm)
        {
            sm.SetBrowser(GetCurrentBrowser()).SetDriverUrl(GetCurrentDriverUrl());
            if (useWatermark)
                sm.Add(new WatermarkService());

            //As of Chrome Driver V 111, remote-allow-origins argument is required, 
            //but Selenium 4.8.2/3 had fixed this breaking change by including this argument
            if (UseHeadlessDriver())
                sm.SetArguments(new string[] { "--headless" });
            if (UseRemoteWebDriver())
                sm.Add(GetRemoteBrowserService());
        }

        private IBrowserService GetRemoteBrowserService()
        {

            // assume that is using remote web driver
            if (UseSelenoidRemoteWebDriver())
                return new SelenoidBrowserService().SetVideo().SetVnc();
            else if (UseGridRemoteWebDriver())
                return new DynamicGridBrowserService().SetVideo().SetVnc();
            else if (UsePreloadLocal())
            {
                string videoContainer = prop.GetProperty("selema.test.preload.video.container");
                string videoSourceFile = prop.GetProperty("selema.test.preload.video.sourcefile");
                string targetFolder = prop.GetProperty("selema.test.preload.video.targetfolder");
                videoSourceFile = CommandLine.IsAbsolute(videoSourceFile) ? videoSourceFile : FileUtil.GetPath(Parameters.DefaultProjectRoot, videoSourceFile);
                targetFolder = CommandLine.IsAbsolute(targetFolder) ? targetFolder : FileUtil.GetPath(Parameters.DefaultProjectRoot, targetFolder);
                VideoControllerLocal controller = new VideoControllerLocal(videoContainer, videoSourceFile, targetFolder);
                return new RemoteBrowserService().SetVideo(controller);
            }
            else if (UsePreloadRemote())
            {
                string label = prop.GetProperty("selema.test.preload.label");
                string videoController = prop.GetProperty("selema.test.preload.video.controller");
                string targetFolder = prop.GetProperty("selema.test.preload.video.targetfolder");
                targetFolder = CommandLine.IsAbsolute(targetFolder) ? targetFolder : FileUtil.GetPath(Parameters.DefaultProjectRoot, targetFolder);
                VideoControllerRemote controller = new VideoControllerRemote(label, videoController, targetFolder);
                return new RemoteBrowserService().SetVideo(controller);
            }
            else
                return null;
        }

        public virtual bool UseSelenoidRemoteWebDriver()
        {
            return "selenoid".Equals(prop.GetProperty("selema.test.mode"));
        }

        public virtual bool UseGridRemoteWebDriver()
        {
            return "grid".Equals(prop.GetProperty("selema.test.mode"));
        }

        public virtual bool UsePreloadLocal()
        {
            return "preload-local".Equals(prop.GetProperty("selema.test.mode"));
        }

        public virtual bool UsePreloadRemote()
        {
            return "preload-remote".Equals(prop.GetProperty("selema.test.mode"));
        }

        public virtual bool UseRemoteWebDriver()
        {
            return UseSelenoidRemoteWebDriver() || UseGridRemoteWebDriver() || UsePreloadLocal() || UsePreloadRemote();
        }

        public virtual bool UseHeadlessDriver()
        {
            return "headless".Equals(prop.GetProperty("selema.test.mode"));
        }

        public virtual bool UseLocalDriver()
        {
            return "local".Equals(prop.GetProperty("selema.test.mode"));
        }

        public virtual string GetRemoteDriverUrl()
        {
            return prop.GetProperty("selema.test.remote.webdriver");
        }

        public virtual string GetCurrentDriverUrl()
        {
            return UseRemoteWebDriver() ? GetRemoteDriverUrl() : "";
        }

        public virtual string GetCurrentBrowser()
        {
            return prop.GetProperty("selema.test.browser");
        }

        public virtual string GetWebRoot()
        {
            return prop.GetProperty("selema.test.web.root");
        }

        public virtual string GetWebUrl()
        {
            return GetWebRoot() + "/main/actions.html";
        }

        public virtual string GetCoverageUrl()
        {
            return GetWebRoot() + "/instrumented/actions.html";
        }

        public virtual string GetCoverageRoot()
        {
            return GetWebRoot() + "/instrumented";
        }

        public virtual bool GetManualCheckEnabled()
        {
            return "true".Equals(prop.GetProperty("selema.test.manual.check"));
        }
    }
}