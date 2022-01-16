package giis.selema.services.impl;

import giis.selema.services.ICiService;

/**
 * Implementation of ICiService used the system is not executed under a supported CI environment
 */
public class LocalService implements ICiService {

	@Override
	public boolean isLocal() {
		return true;
	}
	@Override
	public String getName() {
		return "local";
	}
	@Override
	public String getJobName() {
		return "local-job";
	}
	@Override
	public String getJobId() {
		return "local-job#0";
	}
}
