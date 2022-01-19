package test4giis.selema.core;

import java.util.Properties;

import giis.selema.manager.IManagerConfigDelegate;
import giis.selema.manager.SelemaConfig;
import giis.selema.manager.SeleniumManager;
import giis.selema.portable.FileUtil;
import giis.selema.portable.Parameters;
import giis.selema.portable.PropertiesFactory;
import giis.selema.services.impl.SelenoidService;
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
	public void configure(SeleniumManager sm) {
		sm.setBrowser(getCurrentBrowser()).setDriverUrl(getCurrentDriverUrl());
		if (useWatermark)
			sm.add(new WatermarkService());
		if (useHeadlessDriver())
			sm.setArguments(new String[] { "--headless" });
		if (useRemoteWebDriver())
			sm.add(new SelenoidService().setVideo().setVnc());
	}
	
	public boolean useRemoteWebDriver() {
		return "selenoid".equals(prop.getProperty("selema.test.mode"));
	}
	public boolean useHeadlessDriver() {
		return "headless".equals(prop.getProperty("selema.test.mode"));
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
