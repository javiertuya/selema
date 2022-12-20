using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using NUnit.Framework;

namespace Giis.Samples.Selema.Nunit3
{
    /// <summary>
    /// Selenium Test Lifecycle Manager (selema) sample on the NUnit 3 framework, 
    /// see usage at https://github.com/javiertuya/selema#readme
    /// </summary>
    [LifecycleNunit3]
    public class SampleNunit3
    {
        protected SeleManager sm = new SeleManager().SetBrowser("chrome");
        //More customizations of sm can be made in the OneTimeSetUp method 
        //or in the class constructor (as NUnit only creates a class instance for all tests)

        [Test]
        public void TestFailMethod()
        {
            sm.Driver.Url = "https://javiertuya.github.io/selema/samples/";
            Assert.AreEqual("XXXX Selema samples", sm.Driver.Title);
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
            Assert.AreEqual(expected, sm.Driver.Title);
        }

    }
}