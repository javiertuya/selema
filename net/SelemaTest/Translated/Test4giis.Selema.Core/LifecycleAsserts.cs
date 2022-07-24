/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Giis.Selema.Manager;
using Giis.Selema.Portable;
using Sharpen;

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
			NUnit.Framework.Assert.AreEqual(expected.ToLower(), actual.ToLower());
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
		public virtual void AssertAfterSetup(SeleniumManager sm, bool browserAfter)
		{
			AssertAfterSetup(sm, browserAfter, false);
		}

		public virtual void AssertAfterSetup(SeleniumManager sm, bool browserAfter, bool browserBefore)
		{
			logReader.AssertBegin();
			if (browserBefore)
			{
				CheckBrowserSetup(sm.UsesRemoteDriver());
			}
			logReader.AssertContains("SetUp - " + sm.CurrentTestName());
			if (browserAfter)
			{
				CheckBrowserSetup(sm.UsesRemoteDriver());
			}
			logReader.AssertEnd();
		}

		//checks when simulating failure for failed test in test body
		public virtual void AssertAfterFail(SeleniumManager sm)
		{
			logReader.AssertBegin();
			logReader.AssertContains("FAIL " + sm.CurrentTestName());
			logReader.AssertContains("Taking screenshot", sm.CurrentTestName().Replace(".", "-") + ".png");
			if (sm.UsesRemoteDriver())
			{
				CheckRecordingVideo(sm.GetManageAtClass(), sm.CurrentClassName(), sm.CurrentTestName());
			}
			logReader.AssertEnd();
			CheckScreenShotFile(sm.GetConfig().GetReportDir(), sm.CurrentTestName());
		}

		//videos generated independently if there are any failure, checked in the after teardown callback
		//checks made when lifecycle calls the after tear down callback
		public virtual void AssertAfterTeardown(SeleniumManager sm, string fullTestName, bool success)
		{
			string normalizedTestName = NormalizeTestName(fullTestName);
			logReader.AssertBegin();
			if (success)
			{
				logReader.AssertContains("SUCCESS " + fullTestName);
			}
			else
			{
				logReader.AssertContains("FAIL " + normalizedTestName);
				logReader.AssertContains("Taking screenshot", normalizedTestName.Replace(".", "-"));
				if (sm.UsesRemoteDriver())
				{
					CheckRecordingVideo(!sm.GetManageAtTest(), JavaCs.SplitByDot(fullTestName)[0], normalizedTestName);
				}
			}
			logReader.AssertContains("TearDown - " + fullTestName);
			if (sm.UsesRemoteDriver() && sm.GetManageAtTest())
			{
				logReader.AssertContains("Saving video", normalizedTestName.Replace(".", "-"));
				logReader.AssertContains("Remote session ending");
			}
			logReader.AssertEnd();
			//comprueba el video, en este caso no se trata de un fichero pues estos se guardan en selenoid
			//y solo son copiado tras la ejecucion del job, pero esta el log de los videos
			if (sm.UsesRemoteDriver() && sm.GetManageAtTest())
			{
				IList<string> videoList = FileUtil.FileReadLines(FileUtil.GetPath(sm.GetConfig().GetReportDir(), "video-index.log"));
				string videoPartialName = normalizedTestName.Replace(".", "-");
				logReader.AssertIsTrue(videoList[videoList.Count - 1].Contains(videoPartialName), "No se encuentra el video de nombre " + videoPartialName + " al final del indice");
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
			{
				logReader.AssertContains("Remote" + " session chrome started");
			}
		}

		private void CheckRecordingVideo(bool classLevel, string className, string fullTestName)
		{
			string videoName = (classLevel ? className : fullTestName).Replace(".", "-");
			logReader.AssertContains("Recording video at [00:", videoName);
		}

		private void CheckScreenShotFile(string reportDir, string fullTestName)
		{
			string screenShotName = "screen-*-" + fullTestName.Replace(".", "-") + "*";
			logReader.AssertIsTrue(FileUtil.ListFilesMatchingWildcard(reportDir, screenShotName).Length >= 1, "No hay ningun archivo de nombre " + screenShotName);
		}

		//no =1 pues puede haber de otras ejecucione interactivas
		//Removes leading (...) that may appear in test name when testing repeated tests
		private string NormalizeTestName(string name)
		{
			int position = name.IndexOf('(');
			if (position != -1)
			{
				return JavaCs.Substring(name, 0, position).Trim();
			}
			return name;
		}
	}
}
