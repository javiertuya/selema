using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Giis.Selema.Framework.Mstest2
{
    ///<summary>
    ///Custom extension to allow repeated tests until test passes
    ///</summary>
    public class RetryTestMethod : TestMethodAttribute
    {
        private int maxRepeat;
        public RetryTestMethod(int repetitions)
        {
            maxRepeat = repetitions;
        }
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            for (int i = 1; i < maxRepeat; i++) //repeticiones salvo la ultima, si falla continua
            {
                TestResult[] result = Invoke(testMethod);
                if (result[0].Outcome != UnitTestOutcome.Failed && result[0].Outcome != UnitTestOutcome.Error)
                    return result;
            }
            //last repetition, return the final result at last repetition if previous ones failed (pass or fail)
            return Invoke(testMethod);
        }
        private TestResult[] Invoke(ITestMethod testMethod)
        {
            return new[] { testMethod.Invoke(null) };
        }
    }
}
