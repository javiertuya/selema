package test4giis.selema.core;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.junit.*;

import giis.selema.manager.SelemaConfig;
import giis.selema.manager.SeleManager;

/**
 * Otras pruebas unitarias necesarias, cada uno con su propio selenium manager, 
 * no se sigue el ciclo de vida ni se instancia ningun driver
 */
public class TestLogging {

	@Test
	public void testLoggersAreIndependent() {
		//dos loggers de sm y uno adicional, no se mezclan los mensajes
		//usa la carpeta de log por defecto, por lo que no apareceran bajo selema como el resto de tests.
		SeleManager sm0=new SeleManager().setManagerDelegate(new Config4test());
		LogReader lr0=new LogReader(new SelemaConfig().getReportDir());
		sm0.getLogger().info("test log1 a principal");
		lr0.assertBegin();
		lr0.assertContains("test log1 a principal");
		
		SeleManager sm1=new SeleManager(new SelemaConfig().setName("independent")).setManagerDelegate(new Config4test());
		LogReader lr1=new LogReader(new SelemaConfig().getReportDir(), "selema-log-independent.html");
		sm1.getLogger().info("test log1 a secundario");
		lr1.assertBegin();
		lr1.assertContains("test log1 a secundario");
		lr0.assertBegin();
		lr0.assertEnd(); //no se ha escrito nada aqui
		//compruebo que se puede volver a escribir en el primero
		sm0.getLogger().info("test log2 a principal");
		lr0.assertBegin();
		lr0.assertContains("test log2 a principal");
		lr1.assertBegin();
		lr1.assertEnd(); //no se ha escrito nada aqui
		
		//otro logger no de sm
		Logger lg=LoggerFactory.getLogger("test4giis.selema.core");
		lg.info("test log1 a otro logger");
		lr0.assertBegin();
		lr0.assertEnd();
		lr1.assertBegin();
		lr1.assertEnd();
	}
}
