/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using NLog;
using NUnit.Framework;
using OpenQA.Selenium;
using Sharpen;
using Test4giis.Selema.Core;

namespace Test4giis.Selema.Nunit3
{
	/// <summary>
	/// Comprueba las acciones de test que fallan y pasan, junto con los logs generados que confirman las acciones realizadas
	/// cuando se tiene una sesion de driver por cada uno de los test.
	/// </summary>
	/// <remarks>
	/// Comprueba las acciones de test que fallan y pasan, junto con los logs generados que confirman las acciones realizadas
	/// cuando se tiene una sesion de driver por cada uno de los test.
	/// Comprobar tambien de forma manual:
	/// - las imagenes y videos, incluyendo los enlaces incluidos en el log
	/// - lo anterior desde eclipse y desde el log de Jenkins
	/// </remarks>
	[LifecycleNunit3] public class TestLifecycle4 : IAfterEachCallback
	{
		internal static readonly Logger log = LogManager.GetCurrentClassLogger();

		protected internal static LifecycleAsserts lfas = new LifecycleAsserts();

		//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
		protected internal virtual string CurrentName()
		{
			return "TestLifecycle4";
		}

		protected internal SeleniumManager sm = new SeleniumManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test()).SetManageAtTest();

		
		//public LifecycleNunit3Class cw = new LifecycleNunit3Class(sm);

		
		//public LifecycleNunit3Test tw = new LifecycleNunit3Test(sm, new AfterEachCallback(lfas, log, sm));

		public virtual void RunAfterCallback(string testName, bool success)
		{
			new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
		}

		protected internal virtual void LaunchPage()
		{
			sm.Driver().Url = new Config4test().GetWebUrl();
			//siempre usa la misma pagina
			sm.Watermark();
		}

		/* Uncomment only for debug
		@Before public void setUp() { log.trace("setUp"); }
		@After public void tearDown() { log.trace("tearDown"); }
		@BeforeClass public static void setUpClass() { log.trace("setUpClass"); }
		@AfterClass public static void tearDownClass() { log.trace("tearDownClass"); }
		*/
		[Test]
		public virtual void TestFailMethod()
		{
			sm.GetWatermarkService().SetBackground("yellow");
			lfas.AssertNow(CurrentName() + ".testFailMethod", sm.CurrentTestName());
			lfas.AssertAfterSetup(sm, true);
			LaunchPage();
			//simula un fallo para comprobar luego las acciones realizadas a traves del log
			//NOTA aunque se vera fallo en el watermark, a continuacion se ejecutaran las acciones de teardown correspondientes a no fallo y el test pasara
			sm.OnFailure(CurrentName(), sm.CurrentTestName());
			lfas.AssertAfterFail(sm);
		}

		[Test]
		public virtual void TestPassMethod()
		{
			lfas.AssertNow(CurrentName() + ".testPassMethod", sm.CurrentTestName());
			lfas.AssertAfterSetup(sm, true);
			LaunchPage();
			sm.GetLogger().Info("INSIDE TEST BODY");
			NUnit.Framework.Assert.AreEqual("result", sm.Driver().FindElement(By.Id("spanAlert")).Text);
			lfas.AssertAfterPass();
		}
	}
}
