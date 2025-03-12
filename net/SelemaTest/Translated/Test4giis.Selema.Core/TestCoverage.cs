using NUnit.Framework;
using OpenQA.Selenium;
using Giis.Selema.Manager;
using Giis.Selema.Portable.Selenium;
using Giis.Portable.Util;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Services;
using Giis.Selema.Services.Impl;
using Test4giis.Selema.Portable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Core
{
    public class TestCoverage
    {
        private IWebDriver driver;
        private static IJsCoverageService recorder = JsCoverService.GetInstance(new Config4test().GetCoverageRoot());
        protected internal SeleManager sm = new SeleManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test()).SetManageNone().Add(recorder);
        //public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
        //public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm);
        //Comparara los resultados de cobertura en los siguientes lugares comun para todas las plataformas)
        readonly string TEST_BMK_FOLDER = FileUtil.GetPath(new SelemaConfig().GetProjectRoot(), "..", "java", "src", "test", "resources", "bmk");
        [Test]
        public virtual void TestCoverageAll()
        {
            driver = sm.CreateDriver();
            Asserts.AssertAreEqual("JSCover", DriverUtil.GetTitle(driver), "Page to reset coverage");
            DriverUtil.GetUrl(driver, new Config4test().GetCoverageUrl());

            //sm.watermark();
            RunCoverageSession1(driver);

            //simula la finalizacion de un caso y comienzo de otro cerrando el driver y reabriendolo
            //sin ejecutar nada, deberia tener la cobertura inicial si no se conservase entre sesiones
            //pero tengo esta mas la final pues SessionManager al iniciar por segunda vez el driver
            //ha cargado los ultimos valores de cobertura
            sm.QuitDriver(driver);
            driver = sm.CreateDriver();
            Asserts.AssertAreEqual("Restore JSCover coverage to local storage", DriverUtil.GetTitle(driver), "Page to reload coverage");
            DriverUtil.GetUrl(driver, new Config4test().GetCoverageUrl());
            RunCoverageSession2(driver);
            sm.QuitDriver(driver);

            //simulates failure when no coverage file can be read (the service will become invalidated, name restored in teardown)
            ((JsCoverService)sm.GetCoverageService()).SetCoverageFileName("notexists.json");
            driver = sm.CreateDriver();
            LifecycleAsserts lfas = new LifecycleAsserts();
            lfas.AssertLast("Exception reading js coverage file", "notexists.json");
            sm.QuitDriver(driver);
        }

        //finaliza driver por si ha fallado el anterior pues sm es unmanaged
        [NUnit.Framework.TearDown]
        public virtual void TearDown()
        {
            sm.QuitDriver(driver);
            ((JsCoverService)sm.GetCoverageService()).SetCoverageFileName("jscoverage.json");
        }

        /// <summary>
        /// Ejecucion sucesiva de acciones que aumentan progresivamente la cobertura
        /// </summary>
        private void RunCoverageSession1(IWebDriver driver)
        {
            string CoverageOutFile = FileUtil.GetPath(Config4test.GetConfig().GetReportDir(), "jscoverage.json"); //fichero cobertura generado

            //cobertura inicial por haber cargado la pagina
            recorder.BeforeQuitDriver(driver);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(FileUtil.FileRead(TEST_BMK_FOLDER + "/bmk.initial.jscoverage.json"), FileUtil.FileRead(CoverageOutFile));

            //cobertura intermedia tras ejecutar una accion
            TestActions.RunAlerts(driver, true, false, false);
            recorder.BeforeQuitDriver(driver);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(FileUtil.FileRead(TEST_BMK_FOLDER + "/bmk.middle.jscoverage.json"), FileUtil.FileRead(CoverageOutFile));

            //cobertura final tras ejecutar otra accion
            TestActions.RunAlerts(driver, false, true, true);
            recorder.BeforeQuitDriver(driver);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(FileUtil.FileRead(TEST_BMK_FOLDER + "/bmk.final.jscoverage.json"), FileUtil.FileRead(CoverageOutFile));
        }

        /// <summary>
        /// Ejecucion de una segunda sesion tras haber abandonado la sesion y reabierto,
        /// comprueba que la cobertura de la primera sesion no se pierde
        /// </summary>
        private void RunCoverageSession2(IWebDriver driver)
        {
            string CoverageOutFile = FileUtil.GetPath(Config4test.GetConfig().GetReportDir(), "jscoverage.json"); //fichero cobertura generado
            recorder.BeforeQuitDriver(driver);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(FileUtil.FileRead(TEST_BMK_FOLDER + "/bmk.final2.jscoverage.json"), FileUtil.FileRead(CoverageOutFile));
        }
    }
}