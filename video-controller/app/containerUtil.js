import { CommandLine } from './commandLine.js';

/**
 * Utility class for Docker container operations.
 */
export class ContainerUtil {
  constructor() {
    throw new Error('Utility class');
  }

  /**
   * Runs a docker command (start or stop), throws if output doesn't match container name.
   * @param {string} verb - Docker verb (e.g., 'start', 'stop').
   * @param {string} container - Container name.
   * @returns {Promise<void>}
   */
  static async runDocker(verb, container) {
    const command = `docker ${verb} ${container}`;
    const dockerOut = await CommandLine.runCommand(command);
    if (dockerOut.trim() !== container) {
      throw new Error(`${command} failed. ${dockerOut.trim()}`);
    }
  }

  /**
   * Waits until a line of the container log written since the container was started contains both expected strings.
   * The whole log since the start is checked (not only the last line) because other processes in the container
   * may write after the expected line (e.g. the uploader of the docker-selenium video recorder images).
   * The start time is taken from docker to exclude the log of previous runs of the same container.
   * @param {string} container - Container name.
   * @param {string} expectedLog1 - First expected log string.
   * @param {string} expectedLog2 - Second expected log string.
   * @param {number} timeoutSeconds - Timeout in seconds.
   * @returns {Promise<void>}
   */
  static async waitDocker(container, expectedLog1, expectedLog2, timeoutSeconds) {
    const startedAt = (await CommandLine.runCommand(`docker inspect -f '{{.State.StartedAt}}' ${container}`)).replace(/'/g, '').trim();
    let lastLine = '';
    const maxTries = timeoutSeconds * 10;

    for (let i = 0; i < maxTries; i++) {
      const lines = (await CommandLine.runCommand(`docker logs --since ${startedAt} ${container}`)).split('\n');
      for (const line of lines) {
        if (line.includes(expectedLog1) && line.includes(expectedLog2)) {
          return;
        }
        if (line.trim() !== '') {
          lastLine = line;
        }
      }
      await new Promise(resolve => setTimeout(resolve, 100));
    }

    throw new Error(`Container did not become ready in time, last log message was: ${lastLine}`);
  }

  /**
   * Gets the container status (exited, running, paused).
   * @param {string} name - Container name.
   * @returns {Promise<string>}
   */
  static async getContainerStatus(name) {
    // In java, this does not raises an exception if the container does not exist, but returns the error message.
    // Catch the exception to mimic that behavior.
    try {
      const output = await CommandLine.runCommand(`docker inspect -f '{{.State.Status}}' ${name}`);
      return output.replace(/'/g, '').trim();
    } catch (error) {
      return error.message;
    }
  }
}
