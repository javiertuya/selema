package test4giis.selema.core;
import static org.junit.Assert.assertEquals;

import org.junit.*;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;

import giis.selema.framework.junit4.LifecycleJunit4Class;
import giis.selema.framework.junit4.LifecycleJunit4Test;
import giis.selema.manager.SeleniumManager;
import giis.selema.portable.selenium.SeleniumActions;

public class TestActions { //interface only to generate compatible NUnit3 translation

	protected static SeleniumManager sm=new SeleniumManager(Config4test.getConfig()).setManagerDelegate(new Config4test()).setManageAtClass();
	@ClassRule public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
	@Rule public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm);
	
	@Test
	public void testSendKeys() {
		WebDriver driver=sm.driver();
		driver.get(new Config4test().getWebUrl());
		sm.watermark();
		WebElement elem=driver.findElement(By.id("textbox"));
		SeleniumActions.sendKeysActions(driver, elem, "valueWithActions");
		assertEquals("valueWithActions", elem.getAttribute("value"));
		//ahora con javascript (no requiere clear)
		SeleniumActions.sendKeysJavascript(driver, elem, "valueWithJavascript");
		assertEquals("valueWithJavascript", elem.getAttribute("value"));
	}
	@Test
	public void testSetHtmlOrText() {
		WebDriver driver=sm.driver();
		driver.get(new Config4test().getWebUrl());
		sm.watermark();
		WebElement elem=driver.findElement(By.id("spanSpan"));
		SeleniumActions.setInnerHtml(driver, "spanSpan", "text<strong>Bold</strong>");
		assertEquals("textBold", elem.getText());
		SeleniumActions.setInnerText(driver, "spanSpan", "text<strong>Bold</strong>");
		assertEquals("text<strong>Bold</strong>", elem.getText());
	}
	@Test
	public void testWebDriverWaits() {
		WebDriver driver=sm.driver();
		driver.get(new Config4test().getWebUrl());
		sm.watermark();
		WebElement elem=SeleniumActions.findById(driver, "btnDelay");
		SeleniumActions.clickJavascript(driver, elem);
		WebElement span=driver.findElement(By.id("spanDelay"));
		SeleniumActions.waitUntilTextPresent(driver, span, "TextAfterDelay");
		assertEquals("TextAfterDelay",span.getText());
	}
	/*
	@Test
	public void testAlertsAll() { 
		driver.get(testConfig.getWebUrl());
		watermark();
		runAlerts(true, true, true, 100);
	}
	*/
	
	/**
	 * Ejecucion sucesiva de los distintos tipos de alertas.
	 * Es estatico porque se usa para los test de cobertura js
	 */
	public static void runAlerts(WebDriver driver, boolean doAlert, boolean doConfirm, boolean doPrompt) {
		Boolean waitForClosedAlert=true;

		//Alertas
		if (doAlert) {
			assertEquals("result",driver.findElement(By.id("spanAlert")).getText());
			SeleniumActions.clickActions(driver,driver.findElement(By.id("btnAlert")));
			SeleniumActions.manageAlert(driver,true,false,null,waitForClosedAlert,100); //click ok
			assertEquals("clicked",driver.findElement(By.id("spanAlert")).getText());
		}

		//confirmaciones (si/no)
		if (doConfirm) {
			assertEquals("result",driver.findElement(By.id("spanConfirm")).getText());
			SeleniumActions.clickActions(driver,driver.findElement(By.id("btnConfirm")));
			SeleniumActions.manageAlert(driver,true,false,null,waitForClosedAlert,0); //confirmar
			assertEquals("true",driver.findElement(By.id("spanConfirm")).getText());
			SeleniumActions.clickActions(driver,driver.findElement(By.id("btnConfirm")));
			SeleniumActions.manageAlert(driver,false,true,null,waitForClosedAlert,0); //cancelar
			assertEquals("false",driver.findElement(By.id("spanConfirm")).getText());
		}

		//prompts para introducir datos
		if (doPrompt) {
			assertEquals("result",driver.findElement(By.id("spanPrompt")).getText());
			SeleniumActions.clickActions(driver,driver.findElement(By.id("btnPrompt")));
			SeleniumActions.manageAlert(driver,true,false,null,waitForClosedAlert,0); //confirma sin incluir texto (por poner null)
			assertEquals("",driver.findElement(By.id("spanPrompt")).getText());
			SeleniumActions.clickActions(driver,driver.findElement(By.id("btnPrompt")));
			SeleniumActions.manageAlert(driver,true,false,"text",waitForClosedAlert,0); //confirma incluyendo texto
			assertEquals("text",driver.findElement(By.id("spanPrompt")).getText());
			SeleniumActions.clickActions(driver,driver.findElement(By.id("btnPrompt")));
			SeleniumActions.manageAlert(driver,false,true,"text2",waitForClosedAlert,0); //cancela (incluso habiendo puesto texto)
			assertEquals("NULL",driver.findElement(By.id("spanPrompt")).getText());
		}
	}

}
