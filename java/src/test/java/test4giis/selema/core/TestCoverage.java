package test4giis.selema.core;
import static org.junit.Assert.assertEquals;

import org.junit.*;
import org.openqa.selenium.WebDriver;

import giis.selema.manager.SelemaConfig;
import giis.selema.manager.SeleManager;
import giis.selema.framework.junit4.Asserts;
import giis.selema.framework.junit4.LifecycleJunit4Class;
import giis.selema.framework.junit4.LifecycleJunit4Test;
import giis.selema.portable.FileUtil;
import giis.selema.services.IJsCoverageService;
import giis.selema.services.impl.JsCoverService;

public class TestCoverage {  //interface only to generate compatible NUnit3 translation
	private WebDriver driver;
	
	private static IJsCoverageService recorder=JsCoverService.getInstance(new Config4test().getCoverageRoot());
	protected static SeleManager sm=new SeleManager(Config4test.getConfig()).setManagerDelegate(new Config4test()).setManageNone().add(recorder);
	@ClassRule public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
	@Rule public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm);

	//Comparara los resultados de cobertura en los siguientes lugares comun para todas las plataformas)
	final String TEST_BMK_FOLDER=FileUtil.getPath(new SelemaConfig().getProjectRoot(),"..","java","src","test","resources","bmk"); 
	
	@Test
	public void testCoverageAll() { 
		driver=sm.createDriver();
		Asserts.assertAreEqual("JSCover", driver.getTitle(),"Page to reset coverage");
		driver.get(new Config4test().getCoverageUrl());
		//sm.watermark();
		runCoverageSession1(sm, driver);
		//simula la finalizacion de un caso y comienzo de otro cerrando el driver y reabriendolo
		//sin ejecutar nada, deberia tener la cobertura inicial si no se conservase entre sesiones
		//pero tengo esta mas la final pues SessionManager al iniciar por segunda vez el driver
		//ha cargado los ultimos valores de cobertura
		sm.quitDriver(driver);
		driver=sm.createDriver();
		Asserts.assertAreEqual("Restore JSCover coverage to local storage", driver.getTitle(), "Page to reload coverage");
		driver.get(new Config4test().getCoverageUrl());
		runCoverageSession2(sm, driver);
		sm.quitDriver(driver);
		
		//simulates failure when no coverage file can be read (the service will become invalidated, name restored in teardown)
		((JsCoverService)sm.getCoverageService()).setCoverageFileName("notexists.json");
		driver=sm.createDriver();
		LifecycleAsserts lfas=new LifecycleAsserts();
		lfas.assertLast("Exception reading js coverage file","notexists.json");
		sm.quitDriver(driver);
	}
	//finaliza driver por si ha fallado el anterior pues sm es unmanaged
	@After
	public void tearDown() {
		sm.quitDriver(driver);
		((JsCoverService)sm.getCoverageService()).setCoverageFileName("jscoverage.json");
	}
	/**
	 * Ejecucion sucesiva de acciones que aumentan progresivamente la cobertura
	 */
	private void runCoverageSession1(SeleManager sm, WebDriver driver) {
		String CoverageOutFile=FileUtil.getPath(Config4test.getConfig().getReportDir(),"jscoverage.json"); //fichero cobertura generado
		//cobertura inicial por haber cargado la pagina
		recorder.beforeQuitDriver(driver);
		assertEquals(FileUtil.fileRead(TEST_BMK_FOLDER+"/bmk.initial.jscoverage.json"), FileUtil.fileRead(CoverageOutFile));

		//cobertura intermedia tras ejecutar una accion
		TestActions.runAlerts(driver,true,false,false);
		recorder.beforeQuitDriver(driver);
		assertEquals(FileUtil.fileRead(TEST_BMK_FOLDER+"/bmk.middle.jscoverage.json"), FileUtil.fileRead(CoverageOutFile));

		//cobertura final tras ejecutar otra accion
		TestActions.runAlerts(driver,false,true,true);
		recorder.beforeQuitDriver(driver);
		assertEquals(FileUtil.fileRead(TEST_BMK_FOLDER+"/bmk.final.jscoverage.json"), FileUtil.fileRead(CoverageOutFile));
	}
	/**
	 * Ejecucion de una segunda sesion tras haber abandonado la sesion y reabierto,
	 * comprueba que la cobertura de la primera sesion no se pierde
	 */
	private void runCoverageSession2(SeleManager sm, WebDriver driver) {
		String CoverageOutFile=FileUtil.getPath(Config4test.getConfig().getReportDir(),"jscoverage.json"); //fichero cobertura generado
		recorder.beforeQuitDriver(driver);
		assertEquals(FileUtil.fileRead(TEST_BMK_FOLDER+"/bmk.final2.jscoverage.json"), FileUtil.fileRead(CoverageOutFile));
	}
}
