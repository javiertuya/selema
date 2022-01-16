package giis.selema.manager;

import java.util.Map;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.remote.RemoteWebDriver;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.portable.SelemaException;
import giis.selema.portable.selenium.SeleniumObjects;

/**
 * Instanciacion de driver local y remoto ado el nombre del navegador
 * Aplicable a los drivers para los navegadores de nombre NAME (puede ser minusculas)
 * donde la clase de selenium sigue el patron: org.openqa.selenium.name.NameDriver;
 */
public class SeleniumDriverFactory {
	final Logger log=LoggerFactory.getLogger(this.getClass());
	
	/**
	 * Obtiene el driver local adecuado al browser
	 */
	public WebDriver getLocalSeleniumDriver(String browser, Map<String, Object> options, String[] arguments) {
		return getSeleniumDriver(browser, null, options, arguments);
	}
	
	/**
	 * Obtiene el driver para ejecucion en un navegador a traves del RemoteWebDriver que se encuentra en la url especificada
	 * y con la configuracion requerida para uso de Selenoid
	 */
	public RemoteWebDriver getRemoteSeleniumDriver(String browser, String remoteUrl, Map<String,Object> options, String[] arguments) {
		return (RemoteWebDriver)getSeleniumDriver(browser, remoteUrl, options, arguments);
	}
	
	/**
	 * Crea una instancia del driver correspondiente al browser indicado inicializando las options y arguments 
	 */
	private WebDriver getSeleniumDriver(String browser, String remoteUrl, Map<String, Object> caps, String[] args) {
		SeleniumObjects reflect=new SeleniumObjects();
		try {
			//Establece las capabilities y argumentos en un objeto options
			log.debug("Setting up WebDriver Options, browser: "+browser);
			Object opt=reflect.getOptionsObj(browser, args) ;
			if (caps!=null)
				for (String key: caps.keySet()) //NOSONAR compatibility with .NET
					reflect.setCapability(opt, key, caps.get(key));
			if (args!=null)
				reflect.addArguments(opt, args);
			
			//creacion del driver con las opciones
			log.debug("Setting up WebDriver, browser: "+browser+", url: "+remoteUrl);
			if (remoteUrl==null || "".equals(remoteUrl.trim()))
				return (WebDriver)reflect.getDriverObj(browser, opt);
			else
				return (RemoteWebDriver)reflect.getRemoteDriverObj(remoteUrl, opt);
		} catch (Exception e) { //NOSONAR
			log.error("Can't instantiate object from browser: "+browser+". See debug log for more details");
			throw new SelemaException("Can't instantiate object for browser: "+browser+". Message: "+e.getMessage());
		}
	}
	
	/**
	 * Descarga el driver adecuado al browser
	 */
	public void downloadLocalDriver(String browser) {
		new SeleniumObjects().downloadDriverExecutable(browser);
	}
	
}
