package test4giis.selema.core;

import java.util.Properties;

import giis.portable.util.FileUtil;
import giis.portable.util.Parameters;
import giis.portable.util.PropertiesFactory;
import giis.selema.manager.IManagerConfigDelegate;
import giis.selema.manager.SelemaConfig;
import giis.selema.manager.SeleManager;
import giis.selema.services.IBrowserService;
import giis.selema.services.impl.RemoteBrowserService;
import giis.selema.services.impl.SeleniumGridService;
import giis.selema.services.impl.SelenoidService;
import giis.selema.services.impl.VideoControllerLocal;
import giis.selema.services.impl.WatermarkService;

public class Config4test implements IManagerConfigDelegate {
	private String PROPERTIES_FILE=FileUtil.getPath(new SelemaConfig().getProjectRoot(), "selema.properties");
	private Properties prop;
	private boolean useWatermark;
	public Config4test() {
		this(true);
	}
	public Config4test(boolean useWatermark) {
		this.useWatermark=useWatermark;
		prop=new PropertiesFactory().getPropertiesFromFilename(PROPERTIES_FILE);
		if (prop==null)
			throw new RuntimeException("Can't load test configuration: selema.properties");
	}
	
	//default selema config for tests
	public static SelemaConfig getConfig() {//by default test reports located in subfolder
		return new SelemaConfig().setReportSubdir(FileUtil.getPath(Parameters.DEFAULT_REPORT_SUBDIR + "/selema"));
	}
	
	//implements the interface IManagerConfig to establish the default configuration for test
	public void configure(SeleManager sm) {
		sm.setBrowser(getCurrentBrowser()).setDriverUrl(getCurrentDriverUrl());
		if (useWatermark)
			sm.add(new WatermarkService());
		//As of Chrome Driver V 111, remote-allow-origins argument is required, 
		//but Selenium 4.8.2/3 had fixed this breaking change by including this argument
		if (useHeadlessDriver()) //headless argument supported by chrome and edge (at least)
			sm.setArguments(new String[] { "--headless" });
		if (useRemoteWebDriver())
			sm.add(getRemoteBrowserService());
	}
	
	private IBrowserService getRemoteBrowserService() { // assume that is using remote web driver
		if (useSelenoidRemoteWebDriver())
			return new SelenoidService().setVideo().setVnc();
		else if (useSeleniumRemoteWebDriver())
			return new SeleniumGridService().setVideo().setVnc();
		else if (usePreloadLocal()) {
			String videoContainer = prop.getProperty("selema.test.preload.video.container");
			String videoLocation = prop.getProperty("selema.test.preload.video.location");
			VideoControllerLocal controller = new VideoControllerLocal(videoContainer, videoLocation, "video.mp4");
			return new RemoteBrowserService().setVideo(controller);
		} else
			return null;
	}
	public boolean useSelenoidRemoteWebDriver() {
		return "selenoid".equals(prop.getProperty("selema.test.mode"));
	}
	public boolean useSeleniumRemoteWebDriver() {
		return "grid".equals(prop.getProperty("selema.test.mode"));
	}
	public boolean usePreloadLocal() {
		return "preload-local".equals(prop.getProperty("selema.test.mode"));
	}
	public boolean useRemoteWebDriver() {
		return useSelenoidRemoteWebDriver() || useSeleniumRemoteWebDriver() 
				|| usePreloadLocal();
	}
	public boolean useHeadlessDriver() {
		return "headless".equals(prop.getProperty("selema.test.mode"));
	}
	public boolean useLocalDriver() {
		return "local".equals(prop.getProperty("selema.test.mode"));
	}
	public String getRemoteDriverUrl() {
		return prop.getProperty("selema.test.remote.webdriver");
	}
	public String getCurrentDriverUrl() {
		return useRemoteWebDriver() ? getRemoteDriverUrl() : "";
	}
	public String getCurrentBrowser() {
		return prop.getProperty("selema.test.browser");
	}

	public String getWebRoot() {
		return prop.getProperty("selema.test.web.root");
	}
	public String getWebUrl() {
		return getWebRoot() + "/main/actions.html";
	}
	public String getCoverageUrl() {
		return getWebRoot() + "/instrumented/actions.html";
	}
	public String getCoverageRoot() {
		return getWebRoot() + "/instrumented";
	}
	
	public boolean getManualCheckEnabled() {
		return "true".equals(prop.getProperty("selema.test.manual.check"));
	}

}
