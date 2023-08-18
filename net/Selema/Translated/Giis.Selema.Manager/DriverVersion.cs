/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Sharpen;

namespace Giis.Selema.Manager
{
	/// <summary>Strategies for selecting the driver version to download</summary>
	public class DriverVersion
	{
		public const string MatchBrowser = "match";

		public const string LatestAvailable = "latest";

		public const string SeleniumManager = "selenium";

		public const string Default = MatchBrowser;

		private DriverVersion()
		{
			// try match with browser version
			// use the latest available version
			// use the default SeleniumManager
			throw new InvalidOperationException("Utility class");
		}
	}
}
