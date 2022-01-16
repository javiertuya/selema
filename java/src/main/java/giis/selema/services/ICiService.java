package giis.selema.services;

/**
 * Information about the execution environment (CI or local) and job identification
 */
public interface ICiService {

	/**
	 * Returns true if is running in a local environment or in a non supported CI environment)
	 */
	boolean isLocal();
	
	/**
	 * Returns the name of the CI environment
	 */
	String getName();
	
	/**
	 * Returns the job name as provided by the CI environment
	 */
	String getJobName();

	/**
	 * Returns the job execution unique identifier as provided by the CI environment 
	 */
	String getJobId();
	
}