import { CommandLine } from './commandLine.js';
import { ContainerUtil } from './containerUtil.js';
import { Log } from './log.js';

/**
 * Video controller for local Docker-based recording.
 * Assumes the test and recorder containers share the same VM and filesystem.
 * Any unexpected behavior throws an error to be handled by the caller.
 */
export class VideoControllerLocal {
h
  /**
   * Starts the video recording process by ensuring the container is clean and ready.
   * @returns {Promise<void>}
   */
  async start(videoContainer, sourceFile) {
    // Clean up previous video file to avoid concatenation
    await CommandLine.fileDelete(sourceFile, false);

    // Ensure container is stopped before starting
    const status = await ContainerUtil.getContainerStatus(videoContainer);
    if (status !== 'exited') {
      Log.debug(`Video recorder ${videoContainer} is not stopped, restarting`);
      await ContainerUtil.runDocker('stop', videoContainer);
      await ContainerUtil.waitDocker(videoContainer, 'Shutdown complete', '', 5);
    }

    Log.debug(`Starting video recorder: ${videoContainer}`);
    await ContainerUtil.runDocker('start', videoContainer);
    await ContainerUtil.waitDocker(videoContainer, 'Display', 'is open', 5);
  }

  /**
   * Stop the video recording process by ensuring the container is ready.
   * @returns {Promise<void>}
   */
  async stop(videoContainer) {
    Log.debug(`Stopping video recorder: ${videoContainer}`);
    await ContainerUtil.runDocker('stop', videoContainer);
    await ContainerUtil.waitDocker(videoContainer, 'Shutdown complete', '', 5);
  }
}
