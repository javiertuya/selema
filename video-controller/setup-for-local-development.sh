#!/bin/bash

# Scripts to run tests in local development environment for different values of selema.test.mode in selema.properties

# RUN FROM THE ROOT OF THE CLONED REPOSITORY

###### preload-local (browser node) ###### 

# up:
docker network create grid
mkdir -p $PWD/java/target/preload-local
NODE_IMAGE="selenium/standalone-chrome:4.35.0-20250909" VIDEO_IMAGE="selenium/video:ffmpeg-8.0-20250909" \
LABEL=chrome NETWORK=grid FOLDER=/$PWD/java/target/preload-local PORTS=4444:4444 \
  docker compose -f ./video-controller/docker-compose-preload.yml up -d 
# down:
LABEL=chrome NETWORK=grid FOLDER=/$PWD/java/target/preload-local PORTS=4444:4444 \
  docker compose -f ./video-controller/docker-compose-preload.yml down

###### preload-remote (browser node), also needs the video controller ###### 

# up:
docker network create grid
mkdir -p $PWD/video-controller/app/videos
NODE_IMAGE="selenium/standalone-chrome:4.35.0-20250909" VIDEO_IMAGE="selenium/video:ffmpeg-8.0-20250909" \
LABEL=chrome NETWORK=grid FOLDER=/$PWD/video-controller/app/videos PORTS=4444:4444 \
  docker compose -f ./video-controller/docker-compose-preload.yml up -d 
# down:
LABEL=chrome NETWORK=grid FOLDER=/$PWD/video-controller/app/videos PORTS=4444:4444 \
  docker compose -f ./video-controller/docker-compose-preload.yml down

###### preload-remote and vcmock-remote (video controller) ###### 

docker network create grid
mkdir -p $PWD/video-controller/app/videos
NETWORK=grid FOLDER=/$PWD/video-controller/app/videos PORTS=4449:4449 \
docker compose -f ./video-controller/docker-compose-controller.yml up -d --build

# Alternative for debugging: start video recorder server (same port) without a container
cd video-controller/app
npm install && npm start

###### grid ###### 

# From powershell in windows: don't need expose daemon on tcp://localhost:2375 without TLS
docker stop selenium-dynamic-grid
docker rm selenium-dynamic-grid
docker run -d --name selenium-dynamic-grid -p 4444:4444 --shm-size="2g" `
    -e SE_VNC_VIEW_ONLY=true -e SE_VNC_NO_PASSWORD=true -e SE_NODE_MAX_SESSIONS=8 `
    -v ${PWD}/java/src/test/resources/selenium-dynamic-grid.toml:/opt/selenium/docker.toml `
    -v ${PWD}/java/target/grid-assets:/opt/selenium/assets `
    -v /var/run/docker.sock:/var/run/docker.sock `
    selenium/standalone-docker:4.35.0-20250909
