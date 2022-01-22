using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace Giis.Selema.Framework.Mstest2
{
    ///<summary>
    ///Custom extension to allow repeated tests until test passes
    ///</summary>
    public class RetryTestMethod : TestMethodAttribute
    {
        internal readonly Logger log = LogManager.GetCurrentClassLogger();
        private int maxRepeat;
        public RetryTestMethod(int repetitions)
        {
            maxRepeat = repetitions;
        }
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            for (int i = 1; i < maxRepeat; i++) //repeticiones salvo la ultima, si falla continua
            {
                log.Trace("Entering repetition " + i);
                TestResult[] result = Invoke(testMethod);
                log.Trace("Outcome: " + result[0].Outcome.ToString());
                if (result[0].Outcome != UnitTestOutcome.Failed && result[0].Outcome != UnitTestOutcome.Error)
                    return result;
            }
            log.Trace("Last repetition");
            //last repetition, return the final result at last repetition if previous ones failed (pass or fail)
            return Invoke(testMethod);
        }
        private TestResult[] Invoke(ITestMethod testMethod)
        {
            return new[] { testMethod.Invoke(null) };
        }
    }
}
