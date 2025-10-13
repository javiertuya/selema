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
      throw new Error(`${command} failed. ${dockerOut}`);
    }
  }

  /**
   * Waits until the container log contains both expected strings.
   * @param {string} container - Container name.
   * @param {string} expectedLog1 - First expected log string.
   * @param {string} expectedLog2 - Second expected log string.
   * @param {number} timeoutSeconds - Timeout in seconds.
   * @returns {Promise<void>}
   */
  static async waitDocker(container, expectedLog1, expectedLog2, timeoutSeconds) {
    let logOut = '';
    const maxTries = timeoutSeconds * 10;

    for (let i = 0; i < maxTries; i++) {
      logOut = await CommandLine.runCommand(`docker logs --tail 1 ${container}`);
      if (logOut.includes(expectedLog1) && logOut.includes(expectedLog2)) {
        return;
      }
      await new Promise(resolve => setTimeout(resolve, 100));
    }

    throw new Error(`Container did not become ready in time, last log message was: ${logOut}`);
  }

  /**
   * Gets the container status (exited, running, paused).
   * @param {string} name - Container name.
   * @returns {Promise<string>}
   */
  static async getContainerStatus(name) {
    const output = await CommandLine.runCommand(`docker inspect -f '{{.State.Status}}' ${name}`);
    return output.replace(/'/g, '').trim();
  }
}
