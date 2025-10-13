import { CommandLine } from './commandLine.js';
import { ContainerUtil } from './containerUtil.js';

/**
 * Video controller for local Docker-based recording.
 * Assumes the test and recorder containers share the same VM and filesystem.
 * Any unexpected behavior throws an error to be handled by the caller.
 */
export class VideoControllerLocal {
  #videoContainer;
  #sourceFile;
  #targetFolder;

  /**
   * @param {string} videoContainer - Docker container name used for recording.
   * @param {string} sourceFile - Path to the video file inside the container.
   * @param {string} targetFolder - Destination folder for the video file.
   */
  constructor(videoContainer, sourceFile, targetFolder) {
    this.#videoContainer = videoContainer;
    this.#sourceFile = sourceFile;
    this.#targetFolder = targetFolder;
  }

  /**
   * Starts the video recording process by ensuring the container is clean and ready.
   * @returns {Promise<void>}
   */
  async start() {
    // Clean up previous video file to avoid concatenation
    await CommandLine.fileDelete(this.#sourceFile, false);

    // Ensure container is stopped before starting
    const status = await ContainerUtil.getContainerStatus(this.#videoContainer);
    if (status !== 'exited') {
      console.debug(`Video recorder ${this.#videoContainer} is not stopped, restarting`);
      await ContainerUtil.runDocker('stop', this.#videoContainer);
      await ContainerUtil.waitDocker(this.#videoContainer, 'Shutdown complete', '', 5);
    }

    console.debug(`Starting video recorder: ${this.#videoContainer}`);
    await ContainerUtil.runDocker('start', this.#videoContainer);
    await ContainerUtil.waitDocker(this.#videoContainer, 'Display', 'is open', 5);
  }
}
