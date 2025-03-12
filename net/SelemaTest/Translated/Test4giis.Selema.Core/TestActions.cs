using NUnit.Framework;
using OpenQA.Selenium;
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using Giis.Selema.Portable.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Core
{
    [LifecycleNunit3] public class TestActions
    {
        protected internal SeleManager sm = new SeleManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test()).SetManageAtClass();
        //public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
        //public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm);
        [Test]
        public virtual void TestSendKeys()
        {
            IWebDriver driver = sm.Driver;
            DriverUtil.GetUrl(driver, new Config4test().GetWebUrl());
            sm.Watermark();
            IWebElement elem = driver.FindElement(OpenQA.Selenium.By.Id("textbox"));
            SeleniumActions.SendKeysActions(driver, elem, "valueWithActions");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("valueWithActions", elem.GetAttribute("value"));

            //ahora con javascript (no requiere clear)
            SeleniumActions.SendKeysJavascript(driver, elem, "valueWithJavascript");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("valueWithJavascript", elem.GetAttribute("value"));
        }

        [Test]
        public virtual void TestSetHtmlOrText()
        {
            IWebDriver driver = sm.Driver;
            DriverUtil.GetUrl(driver, new Config4test().GetWebUrl());
            sm.Watermark();
            IWebElement elem = driver.FindElement(OpenQA.Selenium.By.Id("spanSpan"));
            SeleniumActions.SetInnerHtml(driver, "spanSpan", "text<strong>Bold</strong>");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("textBold", elem.Text);
            SeleniumActions.SetInnerText(driver, "spanSpan", "text<strong>Bold</strong>");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("text<strong>Bold</strong>", elem.Text);
        }

        [Test]
        public virtual void TestIWebDriverWaits()
        {
            IWebDriver driver = sm.Driver;
            DriverUtil.GetUrl(driver, new Config4test().GetWebUrl());
            sm.Watermark();
            IWebElement elem = SeleniumActions.FindById(driver, "btnDelay");
            SeleniumActions.ClickJavascript(driver, elem);
            IWebElement span = driver.FindElement(OpenQA.Selenium.By.Id("spanDelay"));
            SeleniumActions.WaitUntilTextPresent(driver, span, "TextAfterDelay");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("TextAfterDelay", span.Text);
        }

        /*
         @Test
         public void testAlertsAll() { 
         DriverUtil.loadUrl(driver, testConfig.getWebUrl());
         watermark();
         runAlerts(true, true, true, 100);
         }
         */
        /// <summary>
        /// Ejecucion sucesiva de los distintos tipos de alertas.
        /// Es estatico porque se usa para los test de cobertura js
        /// </summary>
        public static void RunAlerts(IWebDriver driver, bool doAlert, bool doConfirm, bool doPrompt)
        {
            bool waitForClosedAlert = true;

            //Alertas
            if (doAlert)
            {
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("result", driver.FindElement(OpenQA.Selenium.By.Id("spanAlert")).Text);
                SeleniumActions.ClickActions(driver, driver.FindElement(OpenQA.Selenium.By.Id("btnAlert")));
                SeleniumActions.ManageAlert(driver, true, false, null, waitForClosedAlert, 100); //click ok
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("clicked", driver.FindElement(OpenQA.Selenium.By.Id("spanAlert")).Text);
            }


            //confirmaciones (si/no)
            if (doConfirm)
            {
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("result", driver.FindElement(OpenQA.Selenium.By.Id("spanConfirm")).Text);
                SeleniumActions.ClickActions(driver, driver.FindElement(OpenQA.Selenium.By.Id("btnConfirm")));
                SeleniumActions.ManageAlert(driver, true, false, null, waitForClosedAlert, 0); //confirmar
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("true", driver.FindElement(OpenQA.Selenium.By.Id("spanConfirm")).Text);
                SeleniumActions.ClickActions(driver, driver.FindElement(OpenQA.Selenium.By.Id("btnConfirm")));
                SeleniumActions.ManageAlert(driver, false, true, null, waitForClosedAlert, 0); //cancelar
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("false", driver.FindElement(OpenQA.Selenium.By.Id("spanConfirm")).Text);
            }


            //prompts para introducir datos
            if (doPrompt)
            {
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("result", driver.FindElement(OpenQA.Selenium.By.Id("spanPrompt")).Text);
                SeleniumActions.ClickActions(driver, driver.FindElement(OpenQA.Selenium.By.Id("btnPrompt")));
                SeleniumActions.ManageAlert(driver, true, false, null, waitForClosedAlert, 0); //confirma sin incluir texto (por poner null)
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("", driver.FindElement(OpenQA.Selenium.By.Id("spanPrompt")).Text);
                SeleniumActions.ClickActions(driver, driver.FindElement(OpenQA.Selenium.By.Id("btnPrompt")));
                SeleniumActions.ManageAlert(driver, true, false, "text", waitForClosedAlert, 0); //confirma incluyendo texto
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("text", driver.FindElement(OpenQA.Selenium.By.Id("spanPrompt")).Text);
                SeleniumActions.ClickActions(driver, driver.FindElement(OpenQA.Selenium.By.Id("btnPrompt")));
                SeleniumActions.ManageAlert(driver, false, true, "text2", waitForClosedAlert, 0); //cancela (incluso habiendo puesto texto)
                NUnit.Framework.Legacy.ClassicAssert.AreEqual("NULL", driver.FindElement(OpenQA.Selenium.By.Id("spanPrompt")).Text);
            }
        }
    }
}