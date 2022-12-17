/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using Giis.Selema.Services;
using Giis.Selema.Services.Impl;
using NUnit.Framework;
using Sharpen;

namespace Test4giis.Selema.Core
{
	/// <summary>
	/// Otras pruebas unitarias necesarias, cada uno con su propio selenium manager,
	/// no se sigue el ciclo de vida ni se instancia ningun driver
	/// </summary>
	public class TestUnit
	{
		[Test]
		public virtual void TestReprocessFileName()
		{
			Asserts.AssertAreEqual(MediaContext.ReprocessFileName(null), "noname", "nulo");
			Asserts.AssertAreEqual(MediaContext.ReprocessFileName(string.Empty), "noname", "blanco");
			Asserts.AssertAreEqual(MediaContext.ReprocessFileName("comilla\"blanco enyeñcoma,punto.numero123-question?end"), "comilla-blanco-enye-coma-punto-numero123-question-end", "caracteres especiales");
			string string100 = "x123456789x123456789x123456789x123456789x123456789x123456789x123456789x123456789x123456789x123456789";
			Asserts.AssertAreEqual(MediaContext.ReprocessFileName(string100), string100, "string largo exacto");
			Asserts.AssertAreEqual(MediaContext.ReprocessFileName(string100 + "Sobrante"), string100, "string mas largo");
			Asserts.AssertAreEqual(MediaContext.ReprocessFileName("testParametrizado(40, 5;20 6;10 ) [1]"), "testParametrizado-40--5-20-6-10----1-", "parametrizado real");
		}

		[Test]
		public virtual void TestConfigNames()
		{
			//valor por defecto (seleman)
			NUnit.Framework.Assert.AreEqual("selema", new SelemaConfig().GetName());
			NUnit.Framework.Assert.AreEqual(string.Empty, new SelemaConfig().GetQualifier());
			NUnit.Framework.Assert.AreEqual("selema-log.html", new SelemaConfig().GetLogName());
			//valor no por defecto
			NUnit.Framework.Assert.AreEqual("qqq", new SelemaConfig().SetName("qqq").GetName());
			NUnit.Framework.Assert.AreEqual("-qqq", new SelemaConfig().SetName("qqq").GetQualifier());
			NUnit.Framework.Assert.AreEqual("selema-log-qqq.html", new SelemaConfig().SetName("qqq").GetLogName());
		}

		[Test]
		public virtual void TestMediaFileNames()
		{
			//videos identifed with only two numbers (only one per session), others add an autoincremented id
			SelemaConfig conf = new SelemaConfig();
			IMediaContext ctx = new MediaContext(conf.GetReportDir(), conf.GetQualifier(), 8, 9);
			NUnit.Framework.Assert.AreEqual("video-08-09-Class-Method.mp4", ctx.GetVideoFileName("Class.Method"));
			NUnit.Framework.Assert.AreEqual("screen-08-09-01-Class-Method.png", ctx.GetScreenshotFileName("Class.Method"));
			NUnit.Framework.Assert.AreEqual("diff-08-09-02-Class-Method.html", ctx.GetDiffFileName("Class.Method"));
			NUnit.Framework.Assert.AreEqual("diff-08-09-03-Class-Method.html", ctx.GetDiffFileName("Class.Method"));
			//Named SeleManager
			conf = new SelemaConfig().SetName("xxx");
			ctx = new MediaContext(conf.GetReportDir(), conf.GetQualifier(), 6, 7);
			NUnit.Framework.Assert.AreEqual("video-xxx-06-07-Class-Method.mp4", ctx.GetVideoFileName("Class.Method"));
			NUnit.Framework.Assert.AreEqual("screen-xxx-06-07-01-Class-Method.png", ctx.GetScreenshotFileName("Class.Method"));
			NUnit.Framework.Assert.AreEqual("diff-xxx-06-07-02-Class-Method.html", ctx.GetDiffFileName("Class.Method"));
			NUnit.Framework.Assert.AreEqual("diff-xxx-06-07-03-Class-Method.html", ctx.GetDiffFileName("Class.Method"));
		}
	}
}
