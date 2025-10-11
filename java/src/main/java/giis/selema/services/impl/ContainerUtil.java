package giis.selema.services.impl;

import giis.portable.util.JavaCs;
import giis.selema.manager.SelemaException;
import giis.selema.portable.selenium.CommandLine;

public class ContainerUtil {

	private ContainerUtil() {
		throw new IllegalStateException("Utility class");
	}

	/**
	 * Runs a docker command (start or stop), fails if does not respond with the created container name.
	 */
	public static void runDocker(String verb, String container) {
		String command = "docker " + verb + " " + container;
		String dockerOut = CommandLine.runCommand(command);
		if (!container.equals(dockerOut.trim()))
			throw new SelemaException(command + " failed. " + dockerOut);
	}

	/**
	 * Runs a docker command (start or stop) and waits until the container log contains the expected strings.
	 */
	public static void waitDocker(String container, String expectedLog1, String expectedLog2, int timeoutSeconds) {
		// poll the docker log until expected string found
		String logOut = "";
		for (int i = 0; i < timeoutSeconds * 10; i++) {
			logOut = CommandLine.runCommand("docker logs --tail 1 " + container);
			if (logOut.contains(expectedLog1) && logOut.contains(expectedLog2))
				return;
			JavaCs.sleep(100);
		}
		throw new SelemaException("Container did not become ready in time, last log message was: " + logOut);
	}

	/**
	 * Returns the container status (exited, running or paused), error message if it does not exist
	 */
	public static String getContainerStatus(String name) {
		return CommandLine.runCommand("docker inspect -f '{{.State.Status}}' " + name).replace("'", "").trim();
	}

}
