package giis.selema.manager;

/**
 * A delegate used to provide additional configurations to the SeleniumManager
 */
public interface IManagerConfigDelegate {

		void configure(SeleniumManager sm);
}
