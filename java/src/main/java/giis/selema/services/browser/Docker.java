package giis.selema.services.browser;

import giis.portable.util.JavaCs;
import giis.selema.portable.CommandLine;
import giis.selema.portable.VideoControllerException;

public class Docker {

	private Docker() {
		throw new IllegalStateException("Utility class");
	}

	/**
	 * Runs a docker command (start or stop), fails if does not respond with the created container name.
	 */
	public static void runDocker(String verb, String container) {
		String command = "docker " + verb + " " + container;
		String dockerOut = CommandLine.runCommand(command);
		if (!container.equals(dockerOut.trim()))
			throw new VideoControllerException(command + " failed. " + dockerOut);
	}

	/**
	 * Waits until a line of the container log written since the container was started contains the expected strings.
	 * The whole log since the start is checked (not only the last line) because other processes in the container
	 * may write after the expected line (e.g. the uploader of the docker-selenium video recorder images).
	 * The start time is taken from docker to exclude the log of previous runs of the same container.
	 */
	public static void waitDocker(String container, String expectedLog1, String expectedLog2, int timeoutSeconds) {
		String startedAt = CommandLine.runCommand("docker inspect -f '{{.State.StartedAt}}' " + container).replace("'", "").trim();
		// poll the docker log until expected string found
		String lastLine = "";
		for (int i = 0; i < timeoutSeconds * 10; i++) {
			String[] lines = JavaCs.splitByChar(CommandLine.runCommand("docker logs --since " + startedAt + " " + container), '\n');
			for (String line : lines) {
				if (line.contains(expectedLog1) && line.contains(expectedLog2))
					return;
				if (!"".equals(line.trim()))
					lastLine = line;
			}
			JavaCs.sleep(100);
		}
		throw new VideoControllerException("Container did not become ready in time, last log message was: " + lastLine);
	}

	/**
	 * Returns the container status (exited, running or paused), error message if it does not exist
	 */
	public static String getContainerStatus(String name) {
		return CommandLine.runCommand("docker inspect -f '{{.State.Status}}' " + name).replace("'", "").trim();
	}

}
