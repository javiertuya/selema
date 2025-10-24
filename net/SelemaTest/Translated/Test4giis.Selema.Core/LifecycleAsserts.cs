using Giis.Portable.Util;
using Giis.Selema.Manager;
using Giis.Selema.Portable;
using Test4giis.Selema.Portable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Core
{
    public class LifecycleAsserts
    {
        private LogReader logReader = new LogReader(Config4test.GetConfig().GetReportDir());
        private Config4test testConfig = new Config4test();
        public LifecycleAsserts()
        {
        }

        public virtual LogReader GetLogReader()
        {
            return logReader;
        }

        //assert ignorecase para compatibilidad java .net
        public virtual void AssertNow(string expected, string actual)
        {
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(expected.ToLower(), actual.ToLower());
        }

        //checks the log line located at the offset position from end
        public virtual void AssertLast(int offset, params string[] expected)
        {
            logReader.AssertBegin();
            logReader.AssertContains(expected);
            logReader.AssertEnd(offset);
        }

        //checks only last line of log
        public virtual void AssertLast(params string[] expected)
        {
            AssertLast(0, expected);
        }

        //to check inside the test body at the beginning
        public virtual void AssertAfterSetup(SeleManager sm, bool browserAfter)
        {
            AssertAfterSetup(sm, browserAfter, false);
        }

        public virtual void AssertAfterSetup(SeleManager sm, bool browserAfter, bool browserBefore)
        {
            logReader.AssertBegin();
            if (browserBefore)
                CheckBrowserSetup(sm.UsesRemoteDriver());
            logReader.AssertContains("SetUp - " + sm.CurrentTestName());
            if (browserAfter)
                CheckBrowserSetup(sm.UsesRemoteDriver());
            logReader.AssertEnd();
        }

        //checks when simulating failure for failed test in test body
        public virtual void AssertAfterFail(SeleManager sm)
        {
            logReader.AssertBegin();
            logReader.AssertContains("FAIL " + sm.CurrentTestName());
            logReader.AssertContains("Taking screenshot", sm.CurrentTestName().Replace(".", "-") + ".png");
            if (sm.UsesRemoteDriver())
                CheckRecordingVideo(sm.GetManageAtClass(), sm.CurrentClassName(), sm.CurrentTestName());
            logReader.AssertEnd();
            CheckScreenShotFile(sm.GetConfig().GetReportDir(), sm.CurrentTestName()); //videos generated independently if there are any failure, checked in the after teardown callback
        }

        //checks made when lifecycle calls the after tear down callback
        public virtual void AssertAfterTeardown(SeleManager sm, string fullTestName, bool success)
        {
            string normalizedTestName = NormalizeTestName(fullTestName);
            logReader.AssertBegin();
            if (success)
                logReader.AssertContains("SUCCESS " + fullTestName);
            else
            {
                logReader.AssertContains("FAIL " + normalizedTestName);
                logReader.AssertContains("Taking screenshot", normalizedTestName.Replace(".", "-"));
                if (sm.UsesRemoteDriver())
                    CheckRecordingVideo(!sm.GetManageAtTest(), JavaCs.SplitByDot(fullTestName)[0], normalizedTestName);
            }

            logReader.AssertContains("TearDown - " + fullTestName);
            if (sm.UsesRemoteDriver() && sm.GetManageAtTest())
            {
                logReader.AssertContains("Saving video", normalizedTestName.Replace(".", "-"));
                logReader.AssertContains("Remote session ending");
            }

            logReader.AssertEnd();

            // Check the recorded video, only when driver is managed at the test level
            // This seems flaky if the tests run multiple times without cleaning the selema report folder before
            if (sm.UsesRemoteDriver() && sm.GetManageAtTest())
            {
                string reportDir = sm.GetConfig().GetReportDir();

                // Last name in the video index should match with the current test name
                IList<string> videoList = FileUtil.FileReadLines(FileUtil.GetPath(reportDir, "video-index.log"));
                string lastVideoName = videoList[videoList.Count - 1];
                string videoPartialName = normalizedTestName.Replace(".", "-");
                Asserts.AssertIsTrue(lastVideoName.Contains(videoPartialName), "Can't find a video named " + videoPartialName + " at the end of video-index.log");

                // And the video file must exist, excluding:
                // - selenoid (the server writes to a temp file that is later renamed, so that, at this
                //   moment the final video file is not ready)
                // - grid: the files must be created under a different user, not accesible when runnint tests
                // Note that the browser server must be configured to place the videos at the selema reports folder
                Config4test cfg = new Config4test();
                if (cfg.UseSelenoidRemoteWebDriver() || cfg.UseGridRemoteWebDriver())
                    return;
                Asserts.AssertIsTrue(CommandLine.FileExists(FileUtil.GetPath(reportDir, lastVideoName)), "File " + lastVideoName + " should exist in the selema log folder");
            }
        }

        //successful test
        public virtual void AssertAfterPass()
        {
            logReader.AssertBegin();
            logReader.AssertContains("INSIDE TEST BODY");
            logReader.AssertEnd();
        }

        private void CheckBrowserSetup(bool remoteSession)
        {
            logReader.AssertContains((remoteSession ? "Remote" : "Local") + " session " + testConfig.GetCurrentBrowser() + " starting");
            if (remoteSession)
                logReader.AssertContains("Remote" + " session chrome started");
        }

        private void CheckRecordingVideo(bool classLevel, string className, string fullTestName)
        {
            string videoName = (classLevel ? className : fullTestName).Replace(".", "-");
            logReader.AssertContains("Recording video at [00:", videoName);
        }

        private void CheckScreenShotFile(string reportDir, string fullTestName)
        {
            string screenShotName = "screen-*-" + fullTestName.Replace(".", "-") + "*";
            Asserts.AssertIsTrue(FileUtil.GetFilesMatchingWildcard(reportDir, screenShotName).Count >= 1, "No hay ningun archivo de nombre " + screenShotName); //no =1 pues puede haber de otras ejecucione interactivas
        }

        //Removes leading (...) that may appear in test name when testing repeated tests
        private string NormalizeTestName(string name)
        {
            int position = name.IndexOf('(');
            if (position != -1)
                return JavaCs.Substring(name, 0, position).Trim();
            return name;
        }
    }
}