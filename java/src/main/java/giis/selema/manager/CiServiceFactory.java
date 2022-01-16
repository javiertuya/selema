package giis.selema.manager;

import giis.selema.portable.JavaCs;
import giis.selema.services.ICiService;
import giis.selema.services.impl.GithubService;
import giis.selema.services.impl.JenkinsService;
import giis.selema.services.impl.LocalService;

/**
 * Creation of instances of the appropriate CI service
 */
public class CiServiceFactory {

	/**
	 * Creates of the CI service instance that the system is currently running on
	 */
	public ICiService getCurrent() {
		if (isJenkins())
			return new JenkinsService();
		else if (isGithub())
			return new GithubService();
		else
			return new LocalService();
	}
	public boolean isJenkins() {
		String envVar=JavaCs.getEnvironmentVariable("JENKINS_HOME");
		return envVar!=null && !"".equals(envVar);
	}
	public boolean isGithub() {
		String envVar=JavaCs.getEnvironmentVariable("GITHUB_ACTIONS");
		return envVar!=null && !"true".equals(envVar);
	}
	//Gitlab: GITLAB_CI set to true
}
