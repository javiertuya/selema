package giis.selema.framework.junit4;

import java.util.Objects;

import org.junit.runners.model.Statement;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleniumManager;
 
/**
 * Statement to be used in the JUnit 4 rule that handles repeated tests
 */
public class RepeatedTestStatement extends Statement {
	static final Logger log=LoggerFactory.getLogger(RepeatedTestStatement.class);
	private final SeleniumManager sm;
	private IAfterEachCallback afterCallback;
	private final int retryCount;
	private final Statement statement;
 
	RepeatedTestStatement(SeleniumManager manager, int retryCount, Statement statement, IAfterEachCallback afterCallback)
	{
		this.sm = manager;
		this.afterCallback = afterCallback;
		this.retryCount = retryCount;
		this.statement = statement;
	}
 
	@Override
	public void evaluate() throws Throwable
	{
		String className=sm.currentClassName();
        Throwable caughtThrowable = null;
        // retry logic
        for (int i = 0; i < retryCount; i++) {
            log.trace("Entering repetition "+i);
        	String testName=getNameUntilBracket(sm.currentTestName());
            try {
            	if (i>0)
            		sm.onSetUp(className, testName);
                statement.evaluate();
                return;
            }
            catch (Throwable t) { //NOSONAR does not work if catch Exception
            	sm.onFailure(className, testName);
            	sm.onTearDown(className, testName);
                caughtThrowable = t;
                if (i<retryCount-1) {
                	log.trace("Repeated test aborted: Do not fail completely, but repeat the test");
                	if (afterCallback!=null)
                		afterCallback.runAfterCallback(testName, false);
                }
            }
        }
        log.trace("Repeated test failed after " + retryCount + " failures");
        throw Objects.requireNonNull(caughtThrowable);
	}
	private String getNameUntilBracket(String name) {
		int position=name.indexOf('(');
		if (position!=-1)
			return name.substring(0,position).trim();
		return name;
	}
}
