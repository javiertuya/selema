package giis.selema.manager;

import org.openqa.selenium.WebDriver;

/**
 * A delegate used to provide additional driver configurations just after its creation
 */
public interface IDriverConfigDelegate {

		void configure(WebDriver driver);
}
