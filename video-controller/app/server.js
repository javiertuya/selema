import express from 'express';
import { VideoControllerLocal } from './videoControllerLocal.js'; // Ajusta la ruta si es necesario
import { CommandLine } from './commandLine.js';
import { Log } from './log.js';

/**
 * Video Controller Server that manages video recording via Docker containers.
 * 
 * Provides the endpoints to start the recorder (post), stop and download the video (get) and delete the recorded
 * video to get ready for next session.
 * A path parameter (name) is used to identify the video file (stored under the videos folder) 
 * and the container (with a prefix) used for recording.
 * 
 * The support classes used by this server reproduce the behavior of the Java version of the local video controller,
 * they have been transformed with the help of Copilot from Java to JS/Node.
 */
const app = express();
const PORT = process.env.PORT || 3000;

// Parámetros del controlador de vídeo (puedes parametrizar por entorno si lo deseas)
const videoContainerPrefix = 'selenium-video';
const sourceFolder = './videos';

function sourceFile(name) {
    return `${sourceFolder}/${name}.mp4`;
}
function containerName(name) {
    return `${videoContainerPrefix}-${name}`;
}
function info(req) {
    const ip = req.connection.remoteAddress;
    const ipv4 = ip.startsWith('::ffff:') ? ip.substring(7) : ip;
    Log.info(`${req.method} ${req.params.name} from ${ipv4}`);
}

// Endpoint REST
app.post('/selema-video-controller/:name', async (req, res) => {
  info(req);
  const name = req.params.name;
  const videoController = new VideoControllerLocal();
  try {
    await videoController.start(containerName(name), sourceFile(name));
    res.status(200).json({ message: 'Recording started successfully' });
  } catch (error) {
    Log.error(error.message)
    res.status(500).json({ error: error.message });
  }
});

app.get('/selema-video-controller/:name', async (req, res) => {
  info(req);
  const name = req.params.name;
  const videoController = new VideoControllerLocal();
  const file = sourceFile(name);
  try {
    await videoController.stop(containerName(name));

    const fsSync = await import('fs');
    if (!fsSync.existsSync(file)) {
      const message = `Video file not found after recording: ${name}.mp4 `;
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

app.delete('/selema-video-controller/:name', async (req, res) => {
  info(req);
  const name = req.params.name;
  try {
    await CommandLine.fileDelete(sourceFile(name), true);
    res.status(200).json({ message: 'Video file deleted successfully' });
  } catch (error) {
    Log.error(error.message)
    res.status(500).json({ error: error.message });
  }
});

// Inicio del servidor
app.listen(PORT, () => {
  console.log(`Video service listening on port ${PORT}`);
});
