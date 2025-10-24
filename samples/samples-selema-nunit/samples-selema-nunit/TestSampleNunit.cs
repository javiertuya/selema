using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Giis.Samples.Selema.Nunit
{
    /// <summary>
    /// Selenium Test Lifecycle Manager (selema) sample on the NUnit 3 framework, 
    /// see usage at https://github.com/javiertuya/selema#readme
    /// </summary>
    [LifecycleNunit3]
    public class SampleNunit
    {
        protected SeleManager sm = new SeleManager().SetBrowser("chrome");
        //More customizations of sm can be made in the OneTimeSetUp method 
        //or in the class constructor (as NUnit only creates a class instance for all tests)

        [Test]
        public void TestFailMethod()
        {
            sm.Driver.Url = "https://javiertuya.github.io/selema/samples/";
            // Using the new nunit 4 asserts
            Assert.That(sm.Driver.Title, Is.EqualTo("XXXX Selema samples"));
        }

        //Repeated tests demo, uses a counter to simulate failures
        private static int repetitions = 0;

        [Test]
        [Retry(3)]
        public void TestRepeated()
        {
            repetitions++;
            string expected = "Selema samples";
            if (repetitions < 3) //fails except last repetition
                expected = "XXX " + expected;
            sm.Driver.Url = "https://javiertuya.github.io/selema/samples/";
            // now using the classic nunit 3 asserts
            ClassicAssert.AreEqual(expected, sm.Driver.Title);
        }

    }
}