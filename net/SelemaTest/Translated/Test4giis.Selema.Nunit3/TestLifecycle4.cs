using NUnit.Framework;
using NLog;
using Giis.Portable.Util;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using Giis.Selema.Portable.Selenium;
using Test4giis.Selema.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Nunit3
{
    /// <summary>
    /// Comprueba las acciones de test que fallan y pasan, junto con los logs generados que confirman las acciones realizadas
    /// cuando se tiene una sesion de driver por cada uno de los test.
    /// Comprobar tambien de forma manual:
    /// - las imagenes y videos, incluyendo los enlaces incluidos en el log
    /// - lo anterior desde eclipse y desde el log de Jenkins
    /// </summary>
    [LifecycleNunit3] public class TestLifecycle4 : IAfterEachCallback
    {
        //interfaces not needed by JUnit4, included to generate compatible NUnit3 translation
        static readonly Logger log = LogManager.GetCurrentClassLogger(); //TestLifecycle4));
        protected static LifecycleAsserts lfas = new LifecycleAsserts();
        protected virtual string CurrentName()
        {
            return "TestLifecycle4";
        }

        protected internal SeleManager sm = new SeleManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test()).SetManageAtTest();
        //public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
        //public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm, new AfterEachCallback(lfas, log, sm));
        public virtual void RunAfterCallback(string testName, bool success)
        {
            new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
        }

        protected virtual void LaunchPage()
        {
            DriverUtil.GetUrl(sm.Driver, new Config4test().GetWebUrl()); //siempre usa la misma pagina
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

            //lfas.assertNow(currentName()+".testFailMethod", sm.currentTestName());
            //lfas.assertAfterSetup(sm, true);
            LaunchPage();
            JavaCs.Sleep(3000);

            //simula un fallo para comprobar luego las acciones realizadas a traves del log
            //NOTA aunque se vera fallo en el watermark, a continuacion se ejecutaran las acciones de teardown correspondientes a no fallo y el test pasara
            sm.OnFailure(CurrentName(), sm.CurrentTestName()); //lfas.assertAfterFail(sm);
        }

        [Test]
        public virtual void TestPassMethod()
        {

            //lfas.assertNow(currentName()+".testPassMethod",sm.currentTestName());
            //lfas.assertAfterSetup(sm, true);
            LaunchPage();
            sm.GetLogger().Info("INSIDE TEST BODY");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("result", sm.Driver.FindElement(OpenQA.Selenium.By.Id("spanAlert")).Text); //lfas.assertAfterPass();
        }
    }
}