package giis.selema.portable;

import org.slf4j.Logger;

/**
 * Generic custom exception for compatibility Java/C#
 */
public class SelemaException extends RuntimeException {
	private static final long serialVersionUID = -160856912671600396L;
	public SelemaException(Throwable e) {
        super(e);
    }
    public SelemaException(String message) {
        super(message);
    }
    public SelemaException(String message, Throwable cause) {
        super(message + (cause== null ? "" : ". Caused by: " + cause.toString()), cause);
    }
    public SelemaException(Logger log, String message, Throwable cause) {
        super(message, cause);
    	log.error(message, cause);
    }
}
