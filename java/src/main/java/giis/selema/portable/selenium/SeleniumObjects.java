package giis.selema.portable.selenium;

import java.net.URL;

import org.openqa.selenium.Capabilities;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.portable.SelemaException;
import io.github.bonigarcia.wdm.WebDriverManager;
import io.github.bonigarcia.wdm.config.DriverManagerType;

/**
 * Generic browser independent methods to create a configure a Selenium WebDriver:
 * Uses reflection to obtain the different objects involved in this task.
 */
public class SeleniumObjects {
	final Logger log=LoggerFactory.getLogger(this.getClass());

	public Object getOptionsObj(String browser, String[] args) throws Exception { //NOSONAR
		String className=getSeleniumClassName(browser, "Options");
		return Class.forName(className).getConstructor().newInstance();
	}
	public void setCapability(Object opt, String key, Object value) throws Exception { //NOSONAR
		opt.getClass().getMethod("setCapability", String.class, Object.class).invoke(opt, key, value);
	}
	public void addArguments(Object opt, String[] args) throws Exception { //NOSONAR
		opt.getClass().getMethod("addArguments", String[].class).invoke(opt, new Object[] {args}); //NOSONAR
	}
	public Object getDriverObj(String browser, Object opt) throws Exception { //NOSONAR
		String className=getSeleniumClassName(browser, "Driver");
		return Class.forName(className).getConstructor(opt.getClass()).newInstance(new Object[] {opt}); //NOSONAR
	}
	public Object getRemoteDriverObj(String remoteUrl, Object opt) throws Exception { //NOSONAR
		String className=getSeleniumClassName("remote", "Driver");
		return Class.forName(className).getConstructor(URL.class, Capabilities.class).newInstance(new Object[] {new URL(remoteUrl), (Capabilities)opt}); //NOSONAR
	}
	private String getSeleniumClassName(String browser, String clstype) {
		browser=browser.toLowerCase();
		String pkg="org.openqa.selenium." + browser;
		//Handle exceptions to the class name to be obtained
		//if ("edge".equals(browser))
		//	pkg="com.microsoft.edge.seleniumtools";
		String cls="remote".equals(browser) ? "RemoteWeb" : capitalize(browser);

		String className=pkg + "." + cls + clstype;
		log.debug("Getting instance of class: " + className);
		return className;
	}
	private String capitalize(String input) {
		return input.substring(0, 1).toUpperCase() + input.substring(1);
	}

	/**
	 * Downloads the driver executable for the specified browser using WebDriverManager
	 */
	public void downloadDriverExecutable(String browser) {
        try {	
        	DriverManagerType driverManagerType = DriverManagerType.valueOf(browser.toUpperCase());
			Class.forName(driverManagerType.browserClass());
			WebDriverManager.getInstance(driverManagerType).setup();
		} catch (Throwable e) {
			throw new SelemaException(log, "Can't download driver executable for browser: "+browser, e);
		}
	}
}
