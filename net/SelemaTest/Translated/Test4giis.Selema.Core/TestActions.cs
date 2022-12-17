/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using Giis.Selema.Portable.Selenium;
using NUnit.Framework;
using OpenQA.Selenium;
using Sharpen;

namespace Test4giis.Selema.Core
{
	[LifecycleNunit3] public class TestActions
	{
		protected internal SeleManager sm = new SeleManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test()).SetManageAtClass();

		
		//public LifecycleNunit3Class cw = new LifecycleNunit3Class(sm);

		
		//public LifecycleNunit3Test tw = new LifecycleNunit3Test(sm);

		//interface only to generate compatible NUnit3 translation
		[Test]
		public virtual void TestSendKeys()
		{
			IWebDriver driver = sm.Driver;
			driver.Url = new Config4test().GetWebUrl();
			sm.Watermark();
			IWebElement elem = driver.FindElement(By.Id("textbox"));
			SeleniumActions.SendKeysActions(driver, elem, "valueWithActions");
			NUnit.Framework.Assert.AreEqual("valueWithActions", elem.GetAttribute("value"));
			//ahora con javascript (no requiere clear)
			SeleniumActions.SendKeysJavascript(driver, elem, "valueWithJavascript");
			NUnit.Framework.Assert.AreEqual("valueWithJavascript", elem.GetAttribute("value"));
		}

		[Test]
		public virtual void TestSetHtmlOrText()
		{
			IWebDriver driver = sm.Driver;
			driver.Url = new Config4test().GetWebUrl();
			sm.Watermark();
			IWebElement elem = driver.FindElement(By.Id("spanSpan"));
			SeleniumActions.SetInnerHtml(driver, "spanSpan", "text<strong>Bold</strong>");
			NUnit.Framework.Assert.AreEqual("textBold", elem.Text);
			SeleniumActions.SetInnerText(driver, "spanSpan", "text<strong>Bold</strong>");
			NUnit.Framework.Assert.AreEqual("text<strong>Bold</strong>", elem.Text);
		}

		[Test]
		public virtual void TestWebDriverWaits()
		{
			IWebDriver driver = sm.Driver;
			driver.Url = new Config4test().GetWebUrl();
			sm.Watermark();
			IWebElement elem = SeleniumActions.FindById(driver, "btnDelay");
			SeleniumActions.ClickJavascript(driver, elem);
			IWebElement span = driver.FindElement(By.Id("spanDelay"));
			SeleniumActions.WaitUntilTextPresent(driver, span, "TextAfterDelay");
			NUnit.Framework.Assert.AreEqual("TextAfterDelay", span.Text);
		}

		/*
		@Test
		public void testAlertsAll() {
		driver.get(testConfig.getWebUrl());
		watermark();
		runAlerts(true, true, true, 100);
		}
		*/
		/// <summary>Ejecucion sucesiva de los distintos tipos de alertas.</summary>
		/// <remarks>
		/// Ejecucion sucesiva de los distintos tipos de alertas.
		/// Es estatico porque se usa para los test de cobertura js
		/// </remarks>
		public static void RunAlerts(IWebDriver driver, bool doAlert, bool doConfirm, bool doPrompt)
		{
			bool waitForClosedAlert = true;
			//Alertas
			if (doAlert)
			{
				NUnit.Framework.Assert.AreEqual("result", driver.FindElement(By.Id("spanAlert")).Text);
				SeleniumActions.ClickActions(driver, driver.FindElement(By.Id("btnAlert")));
				SeleniumActions.ManageAlert(driver, true, false, null, waitForClosedAlert, 100);
				//click ok
				NUnit.Framework.Assert.AreEqual("clicked", driver.FindElement(By.Id("spanAlert")).Text);
			}
			//confirmaciones (si/no)
			if (doConfirm)
			{
				NUnit.Framework.Assert.AreEqual("result", driver.FindElement(By.Id("spanConfirm")).Text);
				SeleniumActions.ClickActions(driver, driver.FindElement(By.Id("btnConfirm")));
				SeleniumActions.ManageAlert(driver, true, false, null, waitForClosedAlert, 0);
				//confirmar
				NUnit.Framework.Assert.AreEqual("true", driver.FindElement(By.Id("spanConfirm")).Text);
				SeleniumActions.ClickActions(driver, driver.FindElement(By.Id("btnConfirm")));
				SeleniumActions.ManageAlert(driver, false, true, null, waitForClosedAlert, 0);
				//cancelar
				NUnit.Framework.Assert.AreEqual("false", driver.FindElement(By.Id("spanConfirm")).Text);
			}
			//prompts para introducir datos
			if (doPrompt)
			{
				NUnit.Framework.Assert.AreEqual("result", driver.FindElement(By.Id("spanPrompt")).Text);
				SeleniumActions.ClickActions(driver, driver.FindElement(By.Id("btnPrompt")));
				SeleniumActions.ManageAlert(driver, true, false, null, waitForClosedAlert, 0);
				//confirma sin incluir texto (por poner null)
				NUnit.Framework.Assert.AreEqual(string.Empty, driver.FindElement(By.Id("spanPrompt")).Text);
				SeleniumActions.ClickActions(driver, driver.FindElement(By.Id("btnPrompt")));
				SeleniumActions.ManageAlert(driver, true, false, "text", waitForClosedAlert, 0);
				//confirma incluyendo texto
				NUnit.Framework.Assert.AreEqual("text", driver.FindElement(By.Id("spanPrompt")).Text);
				SeleniumActions.ClickActions(driver, driver.FindElement(By.Id("btnPrompt")));
				SeleniumActions.ManageAlert(driver, false, true, "text2", waitForClosedAlert, 0);
				//cancela (incluso habiendo puesto texto)
				NUnit.Framework.Assert.AreEqual("NULL", driver.FindElement(By.Id("spanPrompt")).Text);
			}
		}
	}
}
