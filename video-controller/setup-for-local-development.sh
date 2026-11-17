#!/bin/bash

# Scripts to run tests in local development environment for different values of selema.test.mode in selema.properties
# The docker-selenium images are set in video-controller/selenium-images.env (export NODE_IMAGE, etc. to override them)

# RUN FROM THE ROOT OF THE CLONED REPOSITORY

###### preload-local (browser node) ###### 

# up:
docker network create grid
mkdir -p $PWD/java/target/preload-local
LABEL=chrome NETWORK=grid FOLDER=/$PWD/java/target/preload-local PORTS=4444:4444 \
  docker compose --env-file ./video-controller/selenium-images.env -f ./video-controller/docker-compose-preload.yml up -d
# down:
LABEL=chrome NETWORK=grid FOLDER=/$PWD/java/target/preload-local PORTS=4444:4444 \
  docker compose -f ./video-controller/docker-compose-preload.yml down

###### preload-remote (browser node), also needs the video controller ###### 

# up:
docker network create grid
mkdir -p $PWD/video-controller/app/videos
LABEL=chrome NETWORK=grid FOLDER=/$PWD/video-controller/app/videos PORTS=4444:4444 \
  docker compose --env-file ./video-controller/selenium-images.env -f ./video-controller/docker-compose-preload.yml up -d
# down:
LABEL=chrome NETWORK=grid FOLDER=/$PWD/video-controller/app/videos PORTS=4444:4444 \
  docker compose -f ./video-controller/docker-compose-preload.yml down

###### preload-remote and vcmock-remote (video controller) ###### 

docker network create grid
mkdir -p $PWD/video-controller/app/videos
NETWORK=grid FOLDER=/$PWD/video-controller/app/videos PORTS=4449:4449 \
  docker compose -f ./video-controller/docker-compose-controller.yml up -d --build

# Alternative to test an image published in GHCR instead of building it from the source.
# Temporarily change the node service as explained in docker-compose-controller.yml (do not commit these changes)
NETWORK=grid FOLDER=/$PWD/video-controller/app/videos PORTS=4449:4449 \
  docker compose -f ./video-controller/docker-compose-controller.yml up -d --pull always
# To go back to the image built from the source, restore the compose file and run the up command above (with --build)

# Alternative for debugging: start video recorder server (same port) without a container
cd video-controller/app
npm install && npm start

###### grid ###### 

# up:
docker network create grid
mkdir -p $PWD/java/target/selema
NETWORK=grid FOLDER=/$PWD/java/target/selema PORTS=4444:4444 \
  docker compose --env-file ./video-controller/selenium-images.env -f ./video-controller/docker-compose-grid.yml up -d
# down:
NETWORK=grid FOLDER=/$PWD/java/target/selema PORTS=4444:4444 \
  docker compose -f ./video-controller/docker-compose-grid.yml down
