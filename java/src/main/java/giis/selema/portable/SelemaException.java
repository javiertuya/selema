package giis.selema.portable;

/**
 * Generic custom exception for compatibility Java/C#
 */
public class SelemaException extends RuntimeException {
    public SelemaException(Throwable e) {
        super(e);
    }
    public SelemaException(String message) {
        super(message);
    }
    public SelemaException(String message, Throwable cause) {
        super(message + (cause== null ? "" : ". Caused by: " + cause.toString()), cause);
    }
}
