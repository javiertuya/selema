package giis.selema.manager;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import org.openqa.selenium.Capabilities;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.remote.RemoteWebDriver;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.portable.selenium.SeleniumObjects;

/**
 * Instanciacion de driver local y remoto ado el nombre del navegador
 * Aplicable a los drivers para los navegadores de nombre NAME (puede ser minusculas)
 * donde la clase de selenium sigue el patron: org.openqa.selenium.name.NameDriver;
 */
public class SeleniumDriverFactory {
	final Logger log=LoggerFactory.getLogger(this.getClass());
	private List<String> driversWithSetupDone=new ArrayList<>(); //to avoid duplicate downloads of local drivers
	private String lastOptionString="";
	
	/**
	 * Unified entry point to instantiate a WebDriver for the indicated browser adding the capabilities and arguments specified;
	 * if the remoteUrl is empty or null returns a WebDriver (downloading the driver executable if needed),
	 * if not, returns a RemoteWebDriver 
	 */
	public WebDriver getSeleniumDriver(String browser, String remoteUrl, Map<String, Object> caps, String[] args, Capabilities optInstance) {
		SeleniumObjects reflect=new SeleniumObjects();
		String objectToInstantiate=""; //to enhance error messages
		String url="";
		try {
			objectToInstantiate="WebDriver Options";
			//Sets capabilities and arguments by create an options object
			log.debug("Setting up WebDriver Options, browser: "+browser);
			Object opt = optInstance==null ? reflect.getOptionsObj(browser, args) : optInstance;
			if (caps!=null)
				for (String key: caps.keySet()) //NOSONAR compatibility with .NET
					reflect.setCapability(opt, key, caps.get(key));
			if (args!=null)
				reflect.addArguments(opt, args);
			
			//Creates either local or remote web driver
			objectToInstantiate="WebDriver";
			log.debug("Setting up WebDriver, browser: "+browser+", url: "+remoteUrl);
			lastOptionString=opt.toString();
			log.trace("Option string: "+lastOptionString.replace("\n", "").replace("\r", ""));
			if (remoteUrl==null || "".equals(remoteUrl.trim())) {
				ensureLocalDriverDownloaded(browser);
				return (WebDriver)reflect.getDriverObj(browser, opt);
			} else {
				objectToInstantiate="RemoteWebDriver";
				url=remoteUrl;
				return (RemoteWebDriver)reflect.getRemoteDriverObj(remoteUrl, opt);
			}
		} catch (Exception e) { //NOSONAR
			throw new SelemaException(log, "Can't instantiate "
					+ objectToInstantiate + " for browser: " + browser 
					+ ("".equals(url)?"":" at url: "+url), e);
		}
	}
	/**
	 * Gets the Options object (as string) that corresponds to the latest driver instantiated (only for testing purposes)
	 */
	public String getLastOptionString() {
		return lastOptionString;
	}
	
	/**
	 * Ensures that the appropriate local driver has been downladed, 
	 */
	public void ensureLocalDriverDownloaded(String browser) {
		browser=browser.toLowerCase();
		if (!driversWithSetupDone.contains(browser)) {
			new SeleniumObjects().downloadDriverExecutable(browser);
			driversWithSetupDone.add(browser);
		}
	}
	
}
