package test4giis.selema.core;
import static org.junit.Assert.assertEquals;

import org.junit.*;

import giis.selema.framework.junit4.Asserts;
import giis.selema.manager.SelemaConfig;
import giis.selema.services.IMediaContext;
import giis.selema.services.impl.MediaContext;

/**
 * Otras pruebas unitarias necesarias, cada uno con su propio selenium manager, 
 * no se sigue el ciclo de vida ni se instancia ningun driver
 */
public class TestUnit {
	
	@Test
	public void testReprocessFileName() {
		Asserts.assertAreEqual(MediaContext.reprocessFileName(null), "noname", "nulo");
		Asserts.assertAreEqual(MediaContext.reprocessFileName(""), "noname", "blanco");
		Asserts.assertAreEqual(MediaContext.reprocessFileName("comilla\"blanco enyeñcoma,punto.numero123-question?end"), "comilla-blanco-enye-coma-punto-numero123-question-end", "caracteres especiales");
		String string100="x123456789x123456789x123456789x123456789x123456789x123456789x123456789x123456789x123456789x123456789";
		Asserts.assertAreEqual(MediaContext.reprocessFileName(string100), string100, "string largo exacto");
		Asserts.assertAreEqual(MediaContext.reprocessFileName(string100+"Sobrante"), string100, "string mas largo");
		Asserts.assertAreEqual(MediaContext.reprocessFileName("testParametrizado(40, 5;20 6;10 ) [1]"), "testParametrizado-40--5-20-6-10----1-", "parametrizado real");
	}
	@Test
	public void testConfigNames() {
		//valor por defecto (seleman)
		assertEquals("selema", new SelemaConfig().getName());
		assertEquals("", new SelemaConfig().getQualifier());
		assertEquals("selema-log.html", new SelemaConfig().getLogName());
		//valor no por defecto
		assertEquals("qqq", new SelemaConfig().setName("qqq").getName());
		assertEquals("-qqq", new SelemaConfig().setName("qqq").getQualifier());
		assertEquals("selema-log-qqq.html", new SelemaConfig().setName("qqq").getLogName());
	}
	@Test
	public void testMediaFileNames() {
		//videos identifed with only two numbers (only one per session), others add an autoincremented id
		SelemaConfig conf=new SelemaConfig();
		IMediaContext ctx=new MediaContext(conf.getReportDir(), conf.getQualifier(), 8, 9);
		assertEquals("video-08-09-Class-Method.mp4", ctx.getVideoFileName("Class.Method"));
		assertEquals("screen-08-09-01-Class-Method.png", ctx.getScreenshotFileName("Class.Method"));
		assertEquals("diff-08-09-02-Class-Method.html", ctx.getDiffFileName("Class.Method"));
		assertEquals("diff-08-09-03-Class-Method.html", ctx.getDiffFileName("Class.Method"));
		//Named SeleniumManager
		conf=new SelemaConfig().setName("xxx");
		ctx=new MediaContext(conf.getReportDir(), conf.getQualifier(),  6, 7);
		assertEquals("video-xxx-06-07-Class-Method.mp4", ctx.getVideoFileName("Class.Method"));
		assertEquals("screen-xxx-06-07-01-Class-Method.png", ctx.getScreenshotFileName("Class.Method"));
		assertEquals("diff-xxx-06-07-02-Class-Method.html", ctx.getDiffFileName("Class.Method"));
		assertEquals("diff-xxx-06-07-03-Class-Method.html", ctx.getDiffFileName("Class.Method"));
	}
	
}
