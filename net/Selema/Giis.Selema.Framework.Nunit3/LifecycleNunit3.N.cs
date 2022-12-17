using Giis.Selema.Manager;
using NLog;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Reflection;

namespace Giis.Selema.Framework.Nunit3
{
    /// <summary>
    /// NUnit 3 extension to watch for the class and test lifecycle events, keep track of the test class and test name
    /// and send the appropriate events to the SeleManager.
    /// Binding of the SeleManager is made using reflection.
    /// See https://github.com/javiertuya/selema#readme for instructions
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LifecycleNunit3 : Attribute, ITestAction
    {
        static readonly Logger log = LogManager.GetLogger("Giis.Selema.Framework.Nunit3.LifecycleNunit3");

        private SeleManager sm;
        private IAfterEachCallback afterCallback;
        private string className = "undefined";
        private string testName = "undefined";

        public ActionTargets Targets
        {
            get { return ActionTargets.Test | ActionTargets.Suite; }
        }

        protected virtual string GetTestName(string fullName)
        {
            //Como nombre usa clase.metodo para no generar nombres demasiado largos y ser similar a java
            string[] testcomp = fullName.Split('.');
            string testname = "";
            if (testcomp.Length >= 2)
                testname = testcomp[testcomp.Length - 2] + "." + testcomp[testcomp.Length - 1];
            return testname;
        }
        protected virtual string GetClassName(string fullName)
        {
            string[] testcomp = fullName.Split('.');
            return testcomp[testcomp.Length - 1];
        }

        // Lifecycle management

        public void BeforeTest(ITest test)
        {
            if (test.IsSuite)
            {
                log.Trace("Lifecycle class begin");
                className = test.Name;

                if (sm==null)
                    sm = FindSeleManagerInstance(test.Fixture);

                //prevents cast exception if test class has not defined any of these interfaces
                if (typeof(IAfterEachCallback).IsAssignableFrom(test.Fixture.GetType()))
                    afterCallback = (IAfterEachCallback)test.Fixture;

                if (sm != null)
                    sm.OnSetUpClass(className);
            }
            else
            {
                log.Trace("Lifecycle test begin");
                testName = className + "." + test.Name;
                if (sm != null)
                    sm.OnSetUp(className, testName);
            }
        }
        public void AfterTest(ITest test)
        {
            if (test.IsSuite)
            {
                log.Trace("Lifecycle class end");
                string clasName = test.Name;
                if (sm != null)
                    sm.OnTearDownClass(clasName, "");
            }
            else
            {
                testName = className + "." + test.Name;
                if (sm!=null)
                {
                    if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
                    {
                        log.Trace("Lifecycle test failed");
                        sm.OnFailure(className, testName);
                    }
                    else
                    {
                        log.Trace("Lifecycle test succeeded");
                        sm.OnSuccess(testName);
                    }
                }
                log.Trace("Lifecycle test end");
                if (sm != null)
                    sm.OnTearDown(className, testName);
                log.Trace("Lifecycle afterTearDown callback");
                if (afterCallback != null)
                    afterCallback.RunAfterCallback(testName, TestContext.CurrentContext.Result.FailCount==0);
            }
        }

        public static string GetStr()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the instance of SeleniumManage that is declared in the test instance
        /// </summary>
        private SeleManager FindSeleManagerInstance(Object testInstance)
        {
            try
            {
                FieldInfo[] smFields = testInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default);
                foreach (FieldInfo field in smFields)
                    if (field.FieldType == typeof(SeleManager))
                    {
                        object smInstance = field.GetValue(testInstance);
                        log.Trace("Instance of SeleManager is bound");
                        return (SeleManager)smInstance;
                    }
                log.Warn("Can't bind an instance of SeleManager");
                return null;
            }
            catch (Exception e)
            {
                log.Warn("Error binding an instance of SeleManager, exception: " + e.Message);
                return null;
            }
        }
    }
}
