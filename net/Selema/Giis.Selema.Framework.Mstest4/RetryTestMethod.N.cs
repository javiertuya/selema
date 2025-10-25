using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Giis.Selema.Framework.Mstest4
{
    ///<summary>
    ///Custom extension to allow repeated tests until test passes (MSTest v4 compatible)
    ///Still available here to facilitate migration, prefer using the native v4 [Repeat] annotation
    ///</summary>
    public class RetryTestMethod : TestMethodAttribute
    {
        internal readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly int maxRepeat;
        public RetryTestMethod(int repetitions, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1) : base(callerFilePath, callerLineNumber)
        {
            maxRepeat = repetitions;
        }
        public override async Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
        {
            for (int i = 1; i < maxRepeat; i++) //repeticiones salvo la ultima, si falla continua
            {
                log.Trace("Entering repetition " + i);
                TestResult[] result = await InvokeAsync(testMethod);
                log.Trace("Outcome: " + result[0].Outcome.ToString());
                if (result[0].Outcome != UnitTestOutcome.Failed && result[0].Outcome != UnitTestOutcome.Error)
                    return result;
            }
            log.Trace("Last repetition");
            //last repetition, return the final result at last repetition if previous ones failed (pass or fail)
            return await InvokeAsync(testMethod);
        }
        private async Task<TestResult[]> InvokeAsync(ITestMethod testMethod)
        {
            return new[] { await testMethod.InvokeAsync(null) };
        }
    }
}
