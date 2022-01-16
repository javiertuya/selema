/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Sharpen;

namespace Giis.Selema.Services
{
	/// <summary>Management of VisualAssert comparisons between long strings, managing the html diff files and links from the log</summary>
	public interface IVisualAssertService
	{
		/// <summary>Configures this service, called on attaching the service to a SeleniumManager</summary>
		IVisualAssertService Configure(ISelemaLogger logger, bool local, string projectRoot, string reportSubdir);

		/// <summary>General assertion, intended to be used from a proxy in the SeleniumManager class</summary>
		void AssertEquals(string expected, string actual, string message, IMediaContext context, string testName);
	}
}
