package giis.selema.framework.junit4;

import org.junit.rules.TestWatcher;
import org.junit.runner.Description;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleManager;

/**
 * JUnit 4 rule to watch for the test lifecycle events, keep track of the test class and test name
 * and send the appropriate events to the SeleManager.
 * Binding of the SeleManager is made by a parameter passed in the instantiation.
 * See https://github.com/javiertuya/selema#readme for instructions
 */
public class LifecycleJunit4Test extends TestWatcher {
	static final Logger log=LoggerFactory.getLogger(LifecycleJunit4Test.class);
	private SeleManager sm;
	private String className="undefined";
	private String methodName="undefined";
	private IAfterEachCallback afterCallback;

	public LifecycleJunit4Test(SeleManager smgr, IAfterEachCallback callback) {
		this.afterCallback=callback;
		sm=smgr;
		log.trace("Instance of SeleManager is bound to test rule");
	}
	public LifecycleJunit4Test(SeleManager smgr) {
		this(smgr, null);
	}
	private String getTestName() {
		return className + "." + methodName;
	}

	//Test lifecycle
	
	@Override
	protected void starting(Description description) {
		log.trace("Lifecycle test begin");
		className=description.getTestClass().getSimpleName();
		methodName=description.getMethodName();
		if (sm!=null)
			sm.onSetUp(className, getTestName());
	}
	@Override
	protected void failed(Throwable e, Description description) {
		log.trace("Lifecycle test failed");
		if (sm!=null) //por si hay pruebas unitarias que no lo han definido o es unmanaged
			sm.onFailure(className, getTestName());
	}
	@Override
	protected void succeeded(Description description) {
		log.trace("Lifecycle test succeeded");
		if (sm!=null)
			sm.onSuccess(getTestName());
	}
	@Override
	public void finished(Description description) {
		log.trace("Lifecycle test end");
		if (sm!=null)
			sm.onTearDown(className, getTestName());
		log.trace("Lifecycle afterTearDown callback");
		if (afterCallback!=null)
			afterCallback.runAfterCallback(getTestName(), true);
	}

}
