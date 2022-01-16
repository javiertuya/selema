package test4giis.selema.core;

import org.openqa.selenium.WebDriver;

import giis.selema.manager.IDriverConfigDelegate;

public class DriverConfigMaximize implements IDriverConfigDelegate {
	public void configure(WebDriver driver) {
		driver.manage().window().maximize();
	}
}
