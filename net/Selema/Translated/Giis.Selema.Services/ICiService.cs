/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Sharpen;

namespace Giis.Selema.Services
{
	/// <summary>Information about the execution environment (CI or local) and job identification</summary>
	public interface ICiService
	{
		/// <summary>Returns true if is running in a local environment or in a non supported CI environment)</summary>
		bool IsLocal();

		/// <summary>Returns the name of the CI environment</summary>
		string GetName();

		/// <summary>Returns the job name as provided by the CI environment</summary>
		string GetJobName();

		/// <summary>Returns the job execution unique identifier as provided by the CI environment</summary>
		string GetJobId();
	}
}
