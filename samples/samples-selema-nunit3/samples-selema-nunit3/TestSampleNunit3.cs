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
        protected SeleniumManager sm = new SeleniumManager().SetBrowser("chrome");
        //More customizations of sm can be made in the OneTimeSetUp method 
        //or in the class constructor (as NUnit only creates a class instance for all tests)

        [Test]
        public void TestFailMethod()
        {
            sm.Driver().Url = "https://en.wikipedia.org/";
            Assert.AreEqual("XXXX Wikipedia, the free encyclopedia", sm.Driver().Title);
        }

        //Repeated tests demo, uses a counter to simulate failures
        private static int repetitions = 0;

        [Test]
        [Retry(3)]
        public void TestRepeated()
        {
            repetitions++;
            string expected = "Wikipedia, the free encyclopedia";
            if (repetitions < 3) //fails except last repetition
                expected = "XXX " + expected;
            sm.Driver().Url = "https://en.wikipedia.org/";
            Assert.AreEqual(expected, sm.Driver().Title);
        }

    }
}