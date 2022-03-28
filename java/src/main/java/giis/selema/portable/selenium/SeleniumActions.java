package giis.selema.portable.selenium;

import java.io.File;
import java.time.Duration;

import org.openqa.selenium.Alert;
import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.OutputType;
import org.openqa.selenium.TakesScreenshot;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.interactions.Actions;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import giis.selema.portable.FileUtil;
import giis.selema.portable.JavaCs;

/*
 * Miscelaneous utilites to perform some common Selenium actions
 */
public class SeleniumActions {
	private static final long DEFAULT_TIMEOUT = 10;
	private static final long HALF_TIMEOUT = 5;
	private static final long ZERO_TIMEOUT = 0;
	private SeleniumActions() {
	    throw new IllegalAccessError("Utility class");
	}

	/**
	 * Stores a screenshot in a file
	 */
	public static void takeScreenshotToFile(WebDriver driver, String fileName) {
		File scrFile = ((TakesScreenshot)driver).getScreenshotAs(OutputType.FILE);
		FileUtil.copyFile(scrFile, new File(fileName));
	}
	/**
	 * Click using Actions
	 */
	public static void clickActions(WebDriver driver, WebElement elem) {
		Actions actions = new Actions(driver); 
		actions.moveToElement(elem).click().perform(); 
	}
	/**
	 * Click using the javascript executor
	 */
	public static void clickJavascript(WebDriver driver, WebElement elem) {
		executeScript(driver, "arguments[0].click();", elem);
	}
	/**
	 * SendKeys using Actions.
	 * Crea un metodo propio porque hay diferencias segun navegadores.
	 * Si se usa driver.click() se pueden producir excepciones como "element is obscured"
	 * -En firefox funciona normalmente, se corresponde con esta implementacion
	 * -En chrome no funciona con algunos elementos de texto (p.e. los de handsontable) por lo que se usara una implementacion distinta.
	 * Ver p.e. al final de: http://stackoverflow.com/questions/31547459/selenium-sendkeys-different-behaviour-for-chrome-firefox-and-safari
	 */
	public static void sendKeysActions(WebDriver driver, WebElement elem, String value) {
		Actions actions = new Actions(driver); //requiere hacer click, si no no se envian datos cuando el campo es vacio (chrome)
		actions.moveToElement(elem).click().sendKeys(value).perform();
	}
	/**
	 * SendKeys using the javascript executor
	 */
	public static void sendKeysJavascript(WebDriver driver, WebElement elem, String value) {
		executeScript(driver, "arguments[0].value = arguments[1];", elem, value);
	}

	public static String executeScript(WebDriver driver, String script) {
		return (String)((JavascriptExecutor) driver).executeScript(script);
	}
	public static String executeScript(WebDriver driver, String script, WebElement elem) {
		return (String)((JavascriptExecutor) driver).executeScript(script,elem);
	}
	public static String executeScript(WebDriver driver, String script, WebElement elem, String value) {
		return (String)((JavascriptExecutor) driver).executeScript(script,elem, value);
	}
	public static void setInnerHtml(WebDriver driver, String id, String value) {
		executeScript(driver, "document.getElementById('" + id + "').innerHTML = '" + value + "';");
	}
	public static void setInnerText(WebDriver driver, String id, String value) {
		executeScript(driver, "document.getElementById('" + id + "').textContent = '" + value + "';");
	}

	private static WebDriverWait newWebDriverWait(WebDriver driver, long seconds) {
		return new WebDriverWait(driver, Duration.ofSeconds(seconds));
	}

    /**
     * Locates an element by the given locator using a Wait until clickable
     * (more conditions https://www.selenium.dev/selenium/docs/api/java/org/openqa/selenium/support/ui/ExpectedConditions.html)
     */
    public static WebElement findBy(WebDriver driver, By locator) {
     	WebDriverWait wait = newWebDriverWait(driver, DEFAULT_TIMEOUT);
    	return wait.until(ExpectedConditions.elementToBeClickable(locator));
    }
    /**
     * Locates an element by id using a Wait until clickable
     */
    public static WebElement findById(WebDriver driver, String id) {
        return findBy(driver, By.id(id));
    }
    /**
     * Waits until the text value of an element contains expected
     */
    public static void waitUntilTextPresent(WebDriver seleniumDriver, WebElement elem, String expected)
    {
        WebDriverWait wait = newWebDriverWait(seleniumDriver, DEFAULT_TIMEOUT);
        wait.withMessage("Expected text not found in element.");
        //En .NET OpenQA.Selenium.Support.UI contiene la definicion de WebDriverWait y ExpectedConditions,
        //pero este ultimo esta deprecated, se debe usar SeleniumExtras
        wait.until(ExpectedConditions.textToBePresentInElement(elem, expected));
    }

	/**
	 * Manages the response to an open pop-up window with a single method: alert, confirm, prompt:
	 * @param driver the WebDriver
	 * @param accept if true, clicks accept
	 * @param dismiss if true, clicks cancel (no compatible with accept)
	 * @param text text to write to a prompt pop-up (null if no text)
	 * @param waitForClosedAlert if true, uses a webdriver wait until alert is closed (recommended)
	 * @param waitTime includes a sleep in milliseconds before performing any acction (0 no time)
	 * @return text returned by the pop-up
	 */
	public static String manageAlert(WebDriver driver, boolean accept, boolean dismiss, String text, boolean waitForClosedAlert, int waitTime) {
		if (waitTime>0)
			JavaCs.sleep(waitTime);
		
		//wait for the alert
		WebDriverWait wait = newWebDriverWait(driver, HALF_TIMEOUT);
		wait.until(ExpectedConditions.alertIsPresent());
		
		if (waitTime>0)
			JavaCs.sleep(waitTime);
		
		//actions
		Alert alert = driver.switchTo().alert();
		String alertText=alert.getText();
		if (text!=null)
			alert.sendKeys(text);
		if (accept)
			alert.accept();
		if (dismiss)
			alert.dismiss();

		if (waitTime>0)
			JavaCs.sleep(waitTime);
		
		//spometimes alert contunes open while this method has finished, waits until alert is closed
		if (waitForClosedAlert) {
			try {
				wait = newWebDriverWait(driver, ZERO_TIMEOUT);
				wait.until(ExpectedConditions.alertIsPresent());
				JavaCs.sleep(1000);
			} catch (Exception e) {
				//expected behaviour, not present
			}
		}
		return alertText;
	}

}
