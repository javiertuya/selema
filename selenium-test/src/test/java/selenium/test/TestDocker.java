package selenium.test;

import static org.junit.jupiter.api.Assertions.assertEquals;

import org.junit.jupiter.api.AfterAll;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import java.net.*;
import org.openqa.selenium.*;
import org.openqa.selenium.chrome.*;
import org.openqa.selenium.remote.*;

import giis.portable.util.JavaCs;

public class TestDocker {

	// Ejecutar antes selenium-docker, en windows con setup.ps1

	private String rdriver = "http://127.0.0.1:4444";
	private String root = "https://giis.uniovi.es";
/*
	  NODE_IMAGE: "selenium/standalone-chrome:144.0-20260202"
		  VIDEO_IMAGE: "selenium/video:ffmpeg-8.0-20260202"
		  GRID_IMAGE: "selenium/standalone-docker:4.40.0-20260202"
		  7 segundos start (11 la primera), 7 segundos end
		  
		latest:
		  7-9 segundos start (11 la primera), 37 segundos end
			
*/
	@Test
	public void test1() throws InterruptedException, MalformedURLException {
		ChromeOptions options = new ChromeOptions();
		options.setCapability("se:recordVideo", true);
		options.setCapability("se:name", "my-video2.mp4");

		for (int i = 0; i < 5; i++) {
			System.out.println("*** Run: " + i);
			long timestamp = JavaCs.currentTimeMillis();
			WebDriver driver = new RemoteWebDriver(new URL(rdriver), options);
			System.out.println("Time to start: " + (JavaCs.currentTimeMillis() - timestamp));

			System.out.println("Begin of test, session id: " + ((RemoteWebDriver) driver).getSessionId());
			driver.get(root);
			Thread.sleep(1000);
			driver.get(root + "/testing");
			Thread.sleep(1000);
			driver.get(root + "/projects");
			Thread.sleep(1000);
			System.out.println("End of test");

			timestamp = JavaCs.currentTimeMillis();
			driver.quit();
			System.out.println("Time to stop: " + (JavaCs.currentTimeMillis() - timestamp));
		}
	}

}
