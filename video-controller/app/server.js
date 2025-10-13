import express from 'express';
import { VideoControllerLocal } from './videoControllerLocal.js'; // Ajusta la ruta si es necesario

const app = express();
const PORT = process.env.PORT || 3000;

// Parámetros del controlador de vídeo (puedes parametrizar por entorno si lo deseas)
const videoContainer = 'my-recorder-container';
const sourceFile = '/videos/session.mp4';
const targetFolder = '/videos/output';

// Instancia del controlador
const videoController = new VideoControllerLocal(videoContainer, sourceFile, targetFolder);

// Endpoint REST
app.post('/start', async (req, res) => {
  try {
    await videoController.start();
    res.status(200).json({ message: 'Recording started successfully' });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// Inicio del servidor
app.listen(PORT, () => {
  console.log(`Video service listening on port ${PORT}`);
});
