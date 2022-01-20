/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Services;
using Giis.Selema.Services.Impl;
using Sharpen;

namespace Giis.Selema.Manager
{
	/// <summary>Creates an instance of the selema html logger</summary>
	public class LogFactory
	{
		//20-jan-2022 removed commented code on factory using standard logger (keep logConfigurator as txt)
		public virtual ISelemaLogger GetLogger(string loggerName, string reportDir, string logFileName)
		{
			return new SelemaLogger(loggerName, reportDir, logFileName);
		}
	}
}
