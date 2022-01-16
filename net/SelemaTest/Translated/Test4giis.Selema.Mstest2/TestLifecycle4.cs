/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Framework.Mstest2;
using Giis.Selema.Manager;
using NLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Sharpen;
using Test4giis.Selema.Core;

namespace Test4giis.Selema.Mstest2
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
	[TestClass] public class TestLifecycle4 : LifecycleMstest2
	{
		internal static readonly Logger log = LogManager.GetCurrentClassLogger();

		protected internal static LifecycleAsserts lfas = new LifecycleAsserts();

		//interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
		protected internal virtual string CurrentName()
		{
			return "TestLifecycle4";
		}

		
          protected internal static SeleniumManager sm;
		public TestLifecycle4() {
        
         sm = LifecycleMstest2.GetManager(sm,Config4test.GetConfig()).SetManagerDelegate(new Config4test()).SetManageAtTest();

		
		} //public LifecycleMstest2Class cw = new LifecycleMstest2Class(sm);

		
		//public LifecycleMstest2Test tw = new LifecycleMstest2Test(sm, new AfterEachCallback(lfas, log, sm));

		
          [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
		public static new void TearDownClass() {
    
			LifecycleMstest2.TearDownClass();
		}
        public override void RunAfterCallback(string testName, bool success)
        
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
		[TestMethod]
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

		[TestMethod]
		public virtual void TestPassMethod()
		{
			lfas.AssertNow(CurrentName() + ".testPassMethod", sm.CurrentTestName());
			lfas.AssertAfterSetup(sm, true);
			LaunchPage();
			sm.GetLogger().Info("INSIDE TEST BODY");
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("result", sm.Driver().FindElement(By.Id("spanAlert")).Text);
			lfas.AssertAfterPass();
		}
	}
}
