package giis.selema.services;

/**
 * Manages the start/stop of the video recording for preloaded browser containers
 */
public interface IVideoController {

	void start();

	void stop(String videoFileName);

}