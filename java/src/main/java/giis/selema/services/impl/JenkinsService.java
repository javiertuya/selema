package giis.selema.services.impl;

import giis.portable.util.JavaCs;
import giis.selema.services.ICiService;

/**
 * Information about the jenkins environment and job identification
 */
public class JenkinsService implements ICiService {

	@Override
	public boolean isLocal() {
		return false;
	}
	@Override
	public String getName() {
		return "Jenkins";
	}
	/**
	 * Returns the job name as provided by the Jenkins environment (in a multibranch pipeline, it includes the branch name)
	 */
	@Override
	public String getJobName() {
		return JavaCs.getEnvironmentVariable("JOB_NAME");
	}
	/**
	 * Returns the job execution unique identifier as provided by the Jenkins environment (includes the job name and build number)
	 */
	@Override
	public String getJobId() {
		return getJobName() + "#" + JavaCs.getEnvironmentVariable("BUILD_NUMBER");
	}
}
