/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Framework.Mstest2;
using Giis.Selema.Manager;
using Giis.Selema.Services.Impl;
using NLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharpen;
using Test4giis.Selema.Core;

namespace Test4giis.Selema.Mstest2
{
	[TestClass] public class TestLifecycle4Repeated : LifecycleMstest2
	{
		internal static readonly Logger log = LogManager.GetCurrentClassLogger();

		protected internal static LifecycleAsserts lfas = new LifecycleAsserts();

		
          protected internal static SeleniumManager sm;
		public TestLifecycle4Repeated() {
        
         sm = LifecycleMstest2.GetManager(sm,Config4test.GetConfig()).SetManagerDelegate(new Config4test()).Add(new WatermarkService()).SetMaximize(true);

		
		} //public LifecycleMstest2Class cw = new LifecycleMstest2Class(sm);

		
		//public LifecycleMstest2Test tw = new LifecycleMstest2Test(sm, new AfterEachCallback(lfas, log, sm));

		
		//public RepeatedTestRule repeatRule = new RepeatedTestRule(sm, new AfterEachCallback(lfas, log, sm));

		
          [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
		public static new void TearDownClass() {
    
			LifecycleMstest2.TearDownClass();
		}
        public override void RunAfterCallback(string testName, bool success)
        
		{
			new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
		}

		[TestInitialize]
		public override void SetUp()
		{
			base.SetUp(); sm.Driver().Url = new Config4test().GetWebUrl();
			sm.Watermark();
		}

		private static int repetitions = 0;

		
		[RetryTestMethod(3)] public virtual void TestRepeated()
		{
			lfas.AssertAfterSetup(sm, true);
			repetitions++;
			if (repetitions < 3)
			{
				//falla salvo la ultima repeticion
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("simulated failure");
			}
			else
			{
				if (new Config4test().GetManualCheckEnabled())
				{
					//siempre falla para comprobacion manual
					Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("Manual Check: salida estandar del test muestra nombres de test y log de grabacion video y screenshot");
				}
			}
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(true);
		}
	}
}
