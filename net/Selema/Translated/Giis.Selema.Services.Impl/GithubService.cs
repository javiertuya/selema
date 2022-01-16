/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Portable;
using Giis.Selema.Services;
using Sharpen;

namespace Giis.Selema.Services.Impl
{
	/// <summary>Information about the GitHub Actions environment and job identification</summary>
	public class GithubService : ICiService
	{
		public virtual bool IsLocal()
		{
			return false;
		}

		public virtual string GetName()
		{
			return "GitHub";
		}

		/// <summary>Returns the job name as provided by the GitHub Actions environment (includes the name of the workflow)</summary>
		public virtual string GetJobName()
		{
			return JavaCs.GetEnvironmentVariable("GITHUB_WORKFLOW") + "/" + JavaCs.GetEnvironmentVariable("GITHUB_JOB");
		}

		/// <summary>
		/// Returns the job execution unique identifier as provided by the GitHub Actions environment
		/// (note that the different combinations derived from the matrix strategy can not be differentiated)
		/// </summary>
		public virtual string GetJobId()
		{
			return GetJobName() + "#" + JavaCs.GetEnvironmentVariable("GITHUB_RUN_NUMBER");
		}
	}
}
