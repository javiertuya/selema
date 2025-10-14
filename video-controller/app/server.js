import express from 'express';
import { VideoControllerLocal } from './videoControllerLocal.js'; // Ajusta la ruta si es necesario
import { CommandLine } from './commandLine.js';
import { Log } from './log.js';

/**
 * Video Controller Server to manage the recordings of sessions in a selenium browser node and a sidecar video recorder.
 *
 * Workflow:
 * - Prerequisite: A selenium browser node with a sidecar video recorder must be running in a docker container.
 *   Both containers must be in the same docker network.
 * - A POST request starts the video recorder container with a name based on a prefix and a label (the path parameter).
 *   The video is recorded to a file named <path parmeter>.mp4 in the videos folder.
 * - A GET request stops the recorder container and streams the recorded video file as a download.
 * - A DELETE request deletes the recorded video file to prepare for the next session.
 * 
 * The support classes used by this server reproduce the behavior of the Java version of the local video controller,
 * they have been transformed with the help of Copilot from Java to JS/Node.
 */
const app = express();
const PORT = process.env.SVR_PORT || 4449;
const PATH = process.env.SVR_PATH || "/selema-video-controller";
const VIDEO_CONTAINER_PREFIX = process.env.SVR_VIDEO_CONTAINER_PREFIX || 'selenium-video';
const sourceFolder = './videos';

function sourceFile(label) {
    return `${sourceFolder}/${label}.mp4`;
}
function containerName(label) {
    return `${VIDEO_CONTAINER_PREFIX}-${label}`;
}
function info(req) {
    const ip = req.connection.remoteAddress;
    const ipv4 = ip.startsWith('::ffff:') ? ip.substring(7) : ip;
    Log.info(`${req.method} ${req.params.label} from ${ipv4}`);
}

app.post(`${PATH}/recording/:label`, async (req, res) => {
  info(req);
  const label = req.params.label;
  const videoController = new VideoControllerLocal();
  try {
    await videoController.start(containerName(label), sourceFile(label));
    res.status(200).json({ message: 'Recording started successfully' });
  } catch (error) {
    Log.error(error.message)
    res.status(500).json({ error: error.message });
  }
});

app.get(`${PATH}/recording/:label`, async (req, res) => {
  info(req);
  const label = req.params.label;
  const videoController = new VideoControllerLocal();
  const file = sourceFile(label);
  try {
    await videoController.stop(containerName(label));

    const fsSync = await import('fs');
    if (!fsSync.existsSync(file)) {
      const message = `Video file not found after recording: ${label}.mp4 `;
      Log.error(message)
      return res.status(404).json({ error: message });
    }
    // Configura cabeceras para descarga
    res.setHeader('Content-Type', 'video/mp4');
    res.setHeader('Content-Disposition', 'attachment; filename="session.mp4"');

    // Stream del archivo
    Log.debug(`Saving recorded video file: ${file}`);
    const fileStream = (await import('fs')).createReadStream(file);
    fileStream.pipe(res);
  } catch (error) {
    Log.error(error.message)
    res.status(500).json({ error: error.message });
  }
});

app.delete(`${PATH}/recording/:label`, async (req, res) => {
  info(req);
  const label = req.params.label;
  try {
    await CommandLine.fileDelete(sourceFile(label), true);
    res.status(200).json({ message: 'Video file deleted successfully' });
  } catch (error) {
    Log.error(error.message)
    res.status(500).json({ error: error.message });
  }
});

// Inicio del servidor
app.listen(PORT, () => {
  console.log(`Video service started at ${new Date().toLocaleString()}`);
  console.log(`Video container prefix: ${VIDEO_CONTAINER_PREFIX}`);
  console.log(`Listening on port ${PORT}, endpoint path ${PATH}`);
});
