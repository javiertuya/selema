/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Framework.Nunit3;
using Giis.Selema.Manager;
using Giis.Selema.Services.Impl;
using NLog;
using NUnit.Framework;
using Sharpen;
using Test4giis.Selema.Core;

namespace Test4giis.Selema.Nunit3
{
	[LifecycleNunit3] public class TestLifecycle4Repeated : IAfterEachCallback
	{
		internal static readonly Logger log = LogManager.GetCurrentClassLogger();

		protected internal static LifecycleAsserts lfas = new LifecycleAsserts();

		protected internal SeleManager sm = new SeleManager(Config4test.GetConfig()).SetManagerDelegate(new Config4test()).Add(new WatermarkService()).SetMaximize(true);

		
		//public LifecycleNunit3Class cw = new LifecycleNunit3Class(sm);

		
		//public LifecycleNunit3Test tw = new LifecycleNunit3Test(sm, new AfterEachCallback(lfas, log, sm));

		
		//public RepeatedTestRule repeatRule = new RepeatedTestRule(sm, new AfterEachCallback(lfas, log, sm));

		public virtual void RunAfterCallback(string testName, bool success)
		{
			new AfterEachCallback(lfas, log, sm).RunAfterCallback(testName, success);
		}

		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			sm.Driver.Url = new Config4test().GetWebUrl();
			sm.Watermark();
		}

		private static int repetitions = 0;

		[Test]
		[Retry(3)] public virtual void TestRepeated()
		{
			lfas.AssertAfterSetup(sm, true);
			repetitions++;
			if (repetitions < 3)
			{
				//falla salvo la ultima repeticion
				NUnit.Framework.Legacy.ClassicAssert.Fail("simulated failure");
			}
			else
			{
				if (new Config4test().GetManualCheckEnabled())
				{
					//siempre falla para comprobacion manual
					NUnit.Framework.Legacy.ClassicAssert.Fail("Manual Check: salida estandar del test muestra nombres de test y log de grabacion video y screenshot");
				}
			}
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(true);
		}
	}
}
