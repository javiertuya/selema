package giis.selema.framework.junit5;

import java.lang.reflect.Field;
import java.util.Optional;

import org.junit.jupiter.api.extension.AfterAllCallback;
import org.junit.jupiter.api.extension.AfterEachCallback;
import org.junit.jupiter.api.extension.BeforeAllCallback;
import org.junit.jupiter.api.extension.BeforeEachCallback;
import org.junit.jupiter.api.extension.ExtensionContext;
import org.junit.jupiter.api.extension.TestInstancePostProcessor;
import org.junit.jupiter.api.extension.TestWatcher;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleniumManager;

/**
 * JUnit 5 extension to watch for the class and test lifecycle events, keep track of the test class and test name
 * and send the appropriate events to the SeleniumManager.
 * Binding of the SeleniumManager is made using reflection.
 * See https://github.com/javiertuya/selema#readme for instructions
 */
public class LifecycleJunit5 implements TestWatcher, TestInstancePostProcessor, BeforeAllCallback, AfterAllCallback, BeforeEachCallback, AfterEachCallback {
	static final Logger log=LoggerFactory.getLogger(LifecycleJunit5.class);
	private SeleniumManager sm;
	private String className="undefined";
	private String methodName="undefined";
	private IAfterEachCallback afterCallback;
	private boolean beforeAllActionsDelayed=false;

	private String getTestName() {
		return className + "." + methodName;
	}
	
	//Class lifecycle
	
	@Override
	public void beforeAll(ExtensionContext context) throws Exception {
        log.trace("Lifecycle class begin");
    	className=context.getDisplayName();
    	beforeAllActionsDelayed=true; //pospones to test instance postprocessor the actions to do here that require SeleniumManager
	}
	
	//executed after beforeAll, here the SeleniumManager is bound
    @Override
    public void postProcessTestInstance(Object testInstance, ExtensionContext context) {
    	log.trace("Lifecycle test instance postprocessor");
    	
        //prevent cast exception if test class has not defined any of these interfaces
    	if (IAfterEachCallback.class.isAssignableFrom(testInstance.getClass()))
    		afterCallback=(IAfterEachCallback)testInstance;
    	if (sm==null)
    		sm=findSeleniumManagerInstance(testInstance);
    	
    	if (beforeAllActionsDelayed && sm!=null) {
    		log.trace("Lifecycle actions that were postponed on beforeAll");
    		sm.onSetUpClass(className);
    	}
    	beforeAllActionsDelayed=false;
    }
    @Override
	public void afterAll(ExtensionContext context) throws Exception {
        log.trace("Lifecycle class end");
		if (sm!=null)
			sm.onTearDownClass(className, "");
	}
	
	//Test lifecycle
	
	@Override
	public void beforeEach(ExtensionContext context) throws Exception {
		log.trace("Lifecycle test begin");
		methodName=context.getDisplayName().replace("()","");
		if (sm!=null)
			sm.onSetUp(className, getTestName());
	}
	@Override
	public void afterEach(ExtensionContext context) throws Exception {
		log.trace("afterEach actions will be triggered on testSuccessful and testFailed");
	}
	
	//TestWatcher callbacks (not implemented testAborted, testDisabled)
	@Override
	public void testFailed(ExtensionContext context, Throwable cause) {
		log.trace("Lifecycle test failed");
		if (sm!=null) //por si hay pruebas unitarias que no lo han definido o es unmanaged
			sm.onFailure(className, getTestName());
		//testFailed and tesSuccessful trigger after the afterEach, we need trigger them before afterEach actions
		//to get the driver open (e.g. when taking screenshots)
		afterEachActions(context);
	}
	@Override
	public void testAborted(ExtensionContext context, Throwable cause) {
		log.trace("Lifecycle test aborted: " + cause.getMessage());
		//repeated test that fails before number of repetitions, should notify failure and quit driver
		if (sm!=null) {
			if ("Do not fail completely, but repeat the test".equals(cause.getMessage()))
				sm.onFailure(className, getTestName());
			sm.onTearDown(className, getTestName());
		}
		log.trace("Lifecycle afterTearDown callback");
		if (afterCallback!=null)
			afterCallback.runAfterCallback(getTestName(), false);
	}
	@Override
	public void testDisabled(ExtensionContext context, Optional<String> reason) {
		log.trace("Lifecycle test disabled: " + reason);
	}
	@Override
	public void testSuccessful(ExtensionContext context) {
		log.trace("Lifecycle test succeeded");
		if (sm!=null)
			sm.getLogger().info("SUCCESS " + getTestName());
		afterEachActions(context);
	}
	private void afterEachActions(ExtensionContext context) { //NOSONAR not needed, but requred by the interface
		log.trace("Lifecycle test end");
		if (sm!=null)
			sm.onTearDown(className, getTestName());
		log.trace("Lifecycle afterTearDown callback");
		if (afterCallback!=null)
			afterCallback.runAfterCallback(getTestName(), true);
	}
	
	public void tearDownClass() {
		if (sm!=null)
			sm.onTearDownClass(className, getTestName());
	}

	/**
	 * Finds the instance of SeleniumManager that is declared in the test instance
	 */
	private SeleniumManager findSeleniumManagerInstance(Object testInstance) {
		return findSeleniumManagerInstance(testInstance, testInstance.getClass());
	}
	private SeleniumManager findSeleniumManagerInstance(Object testInstance, Class<?> targetClass) {
    	// https://www.baeldung.com/java-reflection-class-fields
    	try {
			Field[] smFields=targetClass.getDeclaredFields();
			for (Field field : smFields) {
				if (field.getType().equals(SeleniumManager.class)) {
					field.setAccessible(true); //NOSONAR required to allow private SeleniumManager instances
					Object smInstance=field.get(testInstance);
					return (SeleniumManager)smInstance;
				}
			}
			//not found, search in superclass recursively (sm declaration must be public or protected)
			if (targetClass.getSuperclass()!=null)
				return findSeleniumManagerInstance(testInstance, targetClass.getSuperclass());
			
			log.warn("Can't get an instance of SeleniumManager");
			return null;
		} catch (Exception e) {
			log.warn("Can't get an instance of SeleniumManager, exception: "+e.getMessage());
			return null;
		}
	}

}
