using Microsoft.VisualStudio.TestTools.UnitTesting;
using Giis.Selema.Manager;
using NLog;

namespace Giis.Selema.Framework.Mstest2
{
    /// <summary>
    /// MSTest 2 base class extension to watch for the class and test lifecycle events, keep track of the test class and test name
    /// and send the appropriate events to the SeleManager.
    /// All test classes must derive from this class and propagate the Class/Test Initialize/Cleanup to the corresponding methods defined here.
    /// SeleManager must be created with using the GetManager methods to be bound to this instance
    /// See https://github.com/javiertuya/selema#readme for instructions
    /// </summary>
	public abstract class LifecycleMstest2
	{
        static readonly Logger log = LogManager.GetLogger("Giis.Selema.Framework.Mstest2.LifecycleMstest2");
        private static SeleManager sm = null;
        //On MSTest IAfterEachCallback is not defined, it will be executed by subclass in ClassCleanup;
        protected static string className="undefined";
        protected static string methodName="undefined";
        private static int testCount = 0;

        public TestContext TestContext { get; set; }

        //Call this method from the constructor of a class (using the current sm as parameter)
        //Ensures that this sm is declared in the class and it is instantiated only once
        public static SeleManager GetManager(SeleManager manager)
        {
            sm = manager ?? new SeleManager();
            log.Trace("Instance of SeleManager is bound");
            return sm;
        }
        public static SeleManager GetManager(SeleManager manager, SelemaConfig config)
        {
            sm = manager ?? new SeleManager(config);
            log.Trace("Instance of SeleManager is bound");
            return sm;
        }

        protected virtual string GetTestName()
        {
            return className + "." + methodName;
        }

        private static string GetClassName(TestContext TestContext)
        {
            string[] classNameSplit = TestContext.FullyQualifiedTestClassName.Split('.');
            return classNameSplit[classNameSplit.Length - 1];
        }

        public virtual void RunAfterCallback(string testName, bool success)
        {
        }

        [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
        public static void SetUpClass(TestContext TestContext)
        {
            log.Trace("Lifecycle test begin");
            className = GetClassName(TestContext);
            testCount = 0;
        }
        [TestInitialize]
        public virtual void SetUp()
        {
            log.Trace("Lifecycle test begin");
            className = GetClassName(TestContext);
            methodName = TestContext.TestName;
            if (testCount==0 && sm != null)
                sm.OnSetUpClass(className);
            testCount++;
            if (sm != null)
                sm.OnSetUp(className, GetTestName());
        }
        [TestCleanup]
        public virtual void TearDown()
        {
            if (sm != null)
            {
                if (TestContext.CurrentTestOutcome != UnitTestOutcome.Passed)
                {
                    log.Trace("Lifecycle test failed");
                    sm.OnFailure(className, GetTestName());
                }
                else
                {
                    log.Trace("Lifecycle test succeeded");
                    sm.OnSuccess(GetTestName());
                }
            }
            log.Trace("Lifecycle test end");
            if (sm != null)
                sm.OnTearDown(className, GetTestName());
            log.Trace("Lifecycle afterTearDown callback");
            //not use interface, run something if test class overrides this empty method
            RunAfterCallback(GetTestName(), true);
        }

        //NOTE: ClassCleanupBehavior requires MSTest.TestFramework 2.2.8
        //ClassCleanupBehavior.EndOfClass avoids all cleanups be executed after all testss:
        //https://github.com/microsoft/testfx/issues/580
        [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
        public static void TearDownClass()
        {
            log.Trace("Lifecycle class end");
            if (sm != null)
                sm.OnTearDownClass(className, "");
        }

    }
}
