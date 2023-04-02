package giis.selema.manager;

import org.slf4j.Logger;

public class SelemaException extends RuntimeException {
	private static final long serialVersionUID = -2848837670059731155L;

	public SelemaException(String message) {
		super(message);
	}
    public SelemaException(String message, Throwable cause) {
        super(message, cause);
    }
    public SelemaException(Logger log, String message, Throwable cause) {
        super(message, cause);
    	log.error(message, cause);
    }
}
