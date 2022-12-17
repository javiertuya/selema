package giis.selema.framework.junit4;

import org.junit.rules.TestRule;
import org.junit.runner.Description;
import org.junit.runners.model.Statement;

import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleManager;
 
/**
 * JUnit 4 rule to handle repeated tests
 */
public class RepeatedTestRule implements TestRule {
	private SeleManager sm;
	private IAfterEachCallback afterCallback;
	
	public RepeatedTestRule(SeleManager manager) {
		this(manager, null);
	}
	public RepeatedTestRule(SeleManager manager, IAfterEachCallback afterCallback) {
		this.sm=manager;
		this.afterCallback=afterCallback;
	}
	@Override
	public Statement apply(Statement statement, Description description)
	{
		Statement result = statement;
		RepeatedIfExceptionsTest repeat = description.getAnnotation(RepeatedIfExceptionsTest.class);
		if(repeat != null)
		{
			int retryCount = repeat.repeats();
			result = new RepeatedTestStatement(sm, retryCount, statement, afterCallback);
		}
		return result;
	}
}