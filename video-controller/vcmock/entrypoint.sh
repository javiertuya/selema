#!/bin/bash
# KEEP THIS FILE WITH LINUX LINE ENDINGS
echo "Container starting ."
sleep 0.1
echo "Container starting .."
sleep 0.1
echo "Container starting ..."
sleep 0.1
echo "Container started: Display selenium-chrome:99.0 is open"
cp sample.mp4 /app/videos/mock.mp4

# Captura SIGTERM para ejecutar shutdown
trap './shutdown.sh' SIGTERM

# Mantiene el contenedor vivo
while true; do sleep 1; done