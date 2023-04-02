package giis.selema.services.impl;

import giis.portable.util.JavaCs;
import giis.selema.services.ICiService;

/**
 * Information about the GitHub Actions environment and job identification
 */
public class GithubService implements ICiService {

	@Override
	public boolean isLocal() {
		return false;
	}
	@Override
	public String getName() {
		return "GitHub";
	}
	/**
	 * Returns the job name as provided by the GitHub Actions environment (includes the name of the workflow)
	 */
	@Override
	public String getJobName() {
		return JavaCs.getEnvironmentVariable("GITHUB_WORKFLOW") + "/" + JavaCs.getEnvironmentVariable("GITHUB_JOB");
	}
	/**
	 * Returns the job execution unique identifier as provided by the GitHub Actions environment
	 * (note that the different combinations derived from the matrix strategy can not be differentiated)
	 */
	@Override
	public String getJobId() {
		return getJobName() + "#" + JavaCs.getEnvironmentVariable("GITHUB_RUN_NUMBER");
	}

}
