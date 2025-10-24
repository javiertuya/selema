package test4giis.selema.core;

import static org.junit.Assert.assertEquals;

import java.util.List;

import giis.portable.util.FileUtil;
import giis.portable.util.JavaCs;
import giis.selema.manager.SeleManager;
import giis.selema.portable.CommandLine;
import test4giis.selema.portable.Asserts;

public class LifecycleAsserts {
	private LogReader logReader=new LogReader(Config4test.getConfig().getReportDir());
	private Config4test testConfig=new Config4test();
	public LifecycleAsserts() {
	}

	public LogReader getLogReader() {
		return logReader;
	}
	
	//assert ignorecase para compatibilidad java .net
	public void assertNow(String expected, String actual) {
		assertEquals(expected.toLowerCase(), actual.toLowerCase());
	}
	
	//checks the log line located at the offset position from end
	public void assertLast(int offset, String... expected) {
		logReader.assertBegin();
		logReader.assertContains(expected);
		logReader.assertEnd(offset);
	}
	//checks only last line of log
	public void assertLast(String... expected) {
		assertLast(0, expected);
	}
	
	//to check inside the test body at the beginning
	public void assertAfterSetup(SeleManager sm, boolean browserAfter) {
		assertAfterSetup(sm, browserAfter, false);
	}
	public void assertAfterSetup(SeleManager sm, boolean browserAfter, boolean browserBefore) {
		logReader.assertBegin();
		if (browserBefore)
			checkBrowserSetup(sm.usesRemoteDriver());
		logReader.assertContains("SetUp - "+sm.currentTestName());
		if (browserAfter)
			checkBrowserSetup(sm.usesRemoteDriver());
		logReader.assertEnd();
	}

	//checks when simulating failure for failed test in test body
	public void assertAfterFail(SeleManager sm) {
		logReader.assertBegin();
		logReader.assertContains("FAIL "+sm.currentTestName());
		logReader.assertContains("Taking screenshot", sm.currentTestName().replace(".", "-")+".png");
		if (sm.usesRemoteDriver())
			checkRecordingVideo(sm.getManageAtClass(), sm.currentClassName(), sm.currentTestName());
		logReader.assertEnd();
		checkScreenShotFile(sm.getConfig().getReportDir(), sm.currentTestName());
		//videos generated independently if there are any failure, checked in the after teardown callback
	}

	//checks made when lifecycle calls the after tear down callback
	public void assertAfterTeardown(SeleManager sm, String fullTestName, boolean success) {
		String normalizedTestName=normalizeTestName(fullTestName);
		logReader.assertBegin();
		if (success)
			logReader.assertContains("SUCCESS "+fullTestName);
		else {
			logReader.assertContains("FAIL "+normalizedTestName);
			logReader.assertContains("Taking screenshot", normalizedTestName.replace(".", "-"));
			if (sm.usesRemoteDriver())
				checkRecordingVideo(!sm.getManageAtTest(), JavaCs.splitByDot(fullTestName)[0], normalizedTestName);
		}
		logReader.assertContains("TearDown - "+fullTestName);
		if (sm.usesRemoteDriver() && sm.getManageAtTest()) {
			logReader.assertContains("Saving video", normalizedTestName.replace(".", "-"));
			logReader.assertContains("Remote session ending");
		}
		logReader.assertEnd();
		
		// Check the recorded video, only when driver is managed at the test level
		// This seems flaky if the tests run multiple times without cleaning the selema report folder before
		if (sm.usesRemoteDriver() && sm.getManageAtTest()) {
			String reportDir = sm.getConfig().getReportDir();

			// Last name in the video index should match with the current test name
			List<String> videoList = FileUtil.fileReadLines(FileUtil.getPath(reportDir, "video-index.log"));
			String lastVideoName = videoList.get(videoList.size() - 1);
			String videoPartialName = normalizedTestName.replace(".", "-");
			Asserts.assertIsTrue(lastVideoName.contains(videoPartialName),
					"Can't find a video named " + videoPartialName + " at the end of video-index.log");

			// And the video file must exist, excluding:
			// - selenoid (the server writes to a temp file that is later renamed, so that, at this
			//   moment the final video file is not ready)
			// - grid: the files must be created under a different user, not accesible when runnint tests
			// Note that the browser server must be configured to place the videos at the selema reports folder
			Config4test cfg = new Config4test();
			if (cfg.useSelenoidRemoteWebDriver() || cfg.useGridRemoteWebDriver())
				return;
			Asserts.assertIsTrue(CommandLine.fileExists(FileUtil.getPath(reportDir, lastVideoName)),
					"File " + lastVideoName + " should exist in the selema log folder");
		}
	}
	
	//successful test
	public void assertAfterPass() {
		logReader.assertBegin();
		logReader.assertContains("INSIDE TEST BODY");
		logReader.assertEnd();		
	}

	
	private void checkBrowserSetup(boolean remoteSession) {
		logReader.assertContains((remoteSession?"Remote":"Local") + " session " + testConfig.getCurrentBrowser() + " starting");
		if (remoteSession)
			logReader.assertContains("Remote" + " session chrome started");
	}
	private void checkRecordingVideo(boolean classLevel, String className, String fullTestName) {
		String videoName=(classLevel?className:fullTestName).replace(".", "-");
		logReader.assertContains("Recording video at [00:", videoName);
	}
	private void checkScreenShotFile(String reportDir, String fullTestName) {
		String screenShotName="screen-*-"+fullTestName.replace(".","-")+"*";
		Asserts.assertIsTrue(FileUtil.getFilesMatchingWildcard(reportDir, screenShotName).size() >= 1,
				"No hay ningun archivo de nombre "+screenShotName); //no =1 pues puede haber de otras ejecucione interactivas
	}
	
	//Removes leading (...) that may appear in test name when testing repeated tests
	private String normalizeTestName(String name) {
		int position=name.indexOf('(');
		if (position!=-1)
			return JavaCs.substring(name,0,position).trim();
		return name;
	}

}
