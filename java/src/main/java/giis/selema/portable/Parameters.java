package giis.selema.portable;

/**
 * Platform specific constants
 */
public class Parameters {
	private Parameters() {
		throw new IllegalStateException("Utility class");
	}
	public static final String PLATFORM_NAME="java";
	public static final String DEFAULT_PROJECT_ROOT=".";
	public static final String DEFAULT_REPORT_SUBDIR="target";
	public static final String DEFAULT_JENKINS_PROJECT_ROOT=".";
}
