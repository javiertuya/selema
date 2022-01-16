package test4giis.selema.core;

import org.slf4j.Logger;

import giis.selema.manager.IAfterEachCallback;
import giis.selema.manager.SeleniumManager;

public class AfterEachCallback implements IAfterEachCallback{
	private LifecycleAsserts steps;
	private Logger log;
	private SeleniumManager sm;
	public AfterEachCallback(LifecycleAsserts steps, Logger log, SeleniumManager sm) {
		this.steps=steps;
		this.log=log;
		this.sm=sm;
	}
	@Override
	public void runAfterCallback(String testName, boolean success) {
		log.trace("afterTearDown called");
		steps.assertAfterTeardown(sm, testName, success);
	}

}
