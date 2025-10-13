#!/bin/bash

# run from the root folder to build and start the video controller docker container in your development environment
docker build -t selema-video-controller ./video-controller
docker stop selema-video-controller
docker rm selema-video-controller

# From windows, first set: docker desktop -> settings -> General -> Expose daemon on tcp://localhost:2375 without TLS
# and set DOCKER_SOCKET to the environment variable
# From linux, set DOCKER_SOCKET to the volume mount
DOCKER_SOCKET="-e DOCKER_HOST=tcp://host.docker.internal:2375"
#DOCKER_SOCKET="-v /var/run/docker.sock:/var/run/docker.sock"

docker run -d -p 4449:4449 --name selema-video-controller --restart unless-stopped \
    -v /$PWD/video-controller/app/videos:/app/videos \
    $DOCKER_SOCKET \
    selema-video-controller
docker logs selema-video-controller
docker exec -it selema-video-controller ls -la videos


# To start video recorder server (same port) without a container (for debugging)
cd video-controller/app
npm start video-controller
