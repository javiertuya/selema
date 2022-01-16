/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Portable;
using Giis.Selema.Services;
using Giis.Selema.Services.Impl;
using Sharpen;

namespace Giis.Selema.Manager
{
	/// <summary>Creation of instances of the appropriate CI service</summary>
	public class CiServiceFactory
	{
		/// <summary>Creates of the CI service instance that the system is currently running on</summary>
		public virtual ICiService GetCurrent()
		{
			if (IsJenkins())
			{
				return new JenkinsService();
			}
			else
			{
				if (IsGithub())
				{
					return new GithubService();
				}
				else
				{
					return new LocalService();
				}
			}
		}

		public virtual bool IsJenkins()
		{
			string envVar = JavaCs.GetEnvironmentVariable("JENKINS_HOME");
			return envVar != null && !string.Empty.Equals(envVar);
		}

		public virtual bool IsGithub()
		{
			string envVar = JavaCs.GetEnvironmentVariable("GITHUB_ACTIONS");
			return envVar != null && !"true".Equals(envVar);
		}
		//Gitlab: GITLAB_CI set to true
	}
}
