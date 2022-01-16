package giis.selema.manager;
/**
 * Only for testing: a callback to check the log results that is fired after all tear down actions
 */
public interface IAfterEachCallback {
	void runAfterCallback(String testName, boolean success);
}
