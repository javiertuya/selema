using Giis.Selema.Framework.Mstest2;
using Giis.Selema.Manager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Giis.Samples.Selema.Mstest2
{
    /// <summary>
    /// Selenium Test Lifecycle Manager (selema) sample on the MSTest 2 framework, 
    /// see usage at https://github.com/javiertuya/selema#readme
    /// </summary>
    [TestClass]
    public class TestSampleMstest2 : LifecycleMstest2
    {
        protected static SeleManager sm;
        public TestSampleMstest2()
        {
            sm = LifecycleMstest2.GetManager(sm).SetBrowser("chrome");
        }
        //Note that due to the extensibility restrictions of MSTest, the test class must inherit from the Lifecycle class.
        //Additionally, the instantiation and configuration is made in the class constructor.
        //Method GetManager ensures that there is a unique sm instance in the class and it is bound to the Lifecycle class.
        //Instantiation and configuration can also be done in ClassInitialize as shown below
        /*
        [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
        public static new void SetUpClass(TestContext testContext)
        {
            LifecycleMstest2.SetUpClass(testContext);
            sm = LifecycleMstest2.GetManager(sm).SetBrowser("edge");
        }
        */

        [TestMethod]
        public void TestFailMethod()
        {
            sm.Driver.Url = "https://javiertuya.github.io/selema/samples/";
            Assert.AreEqual("XXXX Selema samples", sm.Driver.Title);
        }

        //Repeated tests demo, uses a counter to simulate failures
        private static int repetitions = 0;

        [RetryTestMethod(3)]
        public void TestRepeated()
        {
            repetitions++;
            string expected = "Selema samples";
            if (repetitions < 3) //fails except last repetition
                expected = "XXX " + expected;
            sm.Driver.Url = "https://javiertuya.github.io/selema/samples/";
            Assert.AreEqual(expected, sm.Driver.Title);
        }

    }
}
