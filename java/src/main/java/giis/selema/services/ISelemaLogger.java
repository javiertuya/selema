package giis.selema.services;

/**
 * Write actions to the Selema log
 */
public interface ISelemaLogger {

	void trace(String message);

	void debug(String message);

	void info(String message);

	void warn(String message);

	void error(String message);

}