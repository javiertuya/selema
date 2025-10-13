package giis.selema.portable.selenium;

public class VideoControllerException extends RuntimeException {
	private static final long serialVersionUID = -2848837670059731155L;

	public VideoControllerException(String message) {
		super(message);
	}
	public VideoControllerException(Throwable e) {
		super(e);
	}
    public VideoControllerException(String message, Throwable cause) {
        super(message, cause);
    }
}
