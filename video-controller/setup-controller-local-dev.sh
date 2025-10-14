#!/bin/bash

#To run tests with with selema.properties set to preload-local

###### preload-local (browser node) ###### 
./video-controller/preload-start.sh chrome grid /$PWD/java/target/preload-local "4444:4444" "root"
# to stop containers: ./video-controller/preload-stop.sh chrome
mkdir -p $PWD/java/target/preload-local
NODE_IMAGE="selenium/standalone-chrome:4.35.0-20250909" VIDEO_IMAGE="selenium/video:ffmpeg-8.0-20250909" \
LABEL=chrome NETWORK=grid FOLDER=/$PWD/java/target/preload-local PORTS=4444:4444 \
  docker compose -f ./video-controller/docker-compose-preload.yml up -d 
# down:
LABEL=chrome NETWORK=grid FOLDER=/$PWD/java/target/preload-local PORTS=4444:4444 \
  docker compose -f ./video-controller/docker-compose-preload.yml down


###### preload-remote (browser node) ###### 
./video-controller/preload-start.sh chrome grid "4444:4444"  "root"
# to stop containers: ./video-controller/preload-stop.sh chrome

mkdir -p $PWD/video-controller/app/videos
NODE_IMAGE="selenium/standalone-chrome:4.35.0-20250909" VIDEO_IMAGE="selenium/video:ffmpeg-8.0-20250909" \
LABEL=chrome NETWORK=grid FOLDER=/$PWD/video-controller/app/videos PORTS=4444:4444 \
  docker compose -f ./video-controller/docker-compose-preload.yml up -d 
# down:
LABEL=chrome NETWORK=grid FOLDER=/$PWD/video-controller/app/videos PORTS=4444:4444 \
  docker compose -f ./video-controller/docker-compose-preload.yml down


###### preload-remote and vcmock-remote (video controller) ###### 
./video-controller/preload-controller.sh ./video-controller grid /$PWD/video-controller/app/videos "4449:4449"  "root" tcp

mkdir -p $PWD/video-controller/app/videos
NETWORK=grid FOLDER=/$PWD/video-controller/app/videos PORTS=4449:4449 \
docker compose -f ./video-controller/docker-compose-controller.yml up -d --build

# don't needed in compose:
# note: on linux, use "socket" instead of "tcp"
# note: on windows, first set: docker desktop -> settings -> General -> Expose daemon on tcp://localhost:2375 without TLS

# Alternative for debugging: start video recorder server (same port) without a container
cd video-controller/app
npm install
npm start
