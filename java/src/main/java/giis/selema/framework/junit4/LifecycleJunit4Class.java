package giis.selema.framework.junit4;

import org.junit.rules.ExternalResource;
import org.junit.runner.Description;
import org.junit.runners.model.Statement;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.manager.SeleManager;

/**
 * JUnit 4 rule to watch for the class lifecycle events, keep track of the test class
 * and send the appropriate events to the SeleManager.
 * Binding of the SeleManager is made by a parameter passed in the instantiation.
 * See https://github.com/javiertuya/selema#readme for instructions
 */
public class LifecycleJunit4Class extends ExternalResource {
	static final Logger log=LoggerFactory.getLogger(LifecycleJunit4Class.class);
	private SeleManager sm;
	private String className="undefined";

	public LifecycleJunit4Class(SeleManager smgr) {
		sm=smgr;
		log.trace("Instance of SeleManager is bound to class rule");
	}
	
    public Statement apply(Statement base, Description description) {
    	className=description.getTestClass().getSimpleName();
        return super.apply(base,description);
    }

	@Override
	public void before() throws Throwable {
        log.trace("Lifecycle class begin");
		if (sm!=null)
			sm.onSetUpClass(className);
	}

    @Override
	public void after() {
        log.trace("Lifecycle class end");
		if (sm!=null)
			sm.onTearDownClass(className, "");
	}

}
