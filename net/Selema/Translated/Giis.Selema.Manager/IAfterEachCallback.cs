/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Sharpen;

namespace Giis.Selema.Manager
{
	/// <summary>Only for testing: a callback to check the log results that is fired after all tear down actions</summary>
	public interface IAfterEachCallback
	{
		void RunAfterCallback(string testName, bool success);
	}
}
