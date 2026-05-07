#!/bin/bash

# Check preloaded images across versions

docker network create grid
# This works, stop containers in less than 2 seconds
sudo  \
  NODE_IMAGE="selenium/standalone-chrome:144.0-20260202" VIDEO_IMAGE="selenium/video:ffmpeg-8.0-20260202" \
  FOLDER=$PWD FILE=20260202 USER=root docker compose -f compose-test.yml up -d  --quiet-pull
sleep 5
before=$(($(date +%s%N)/1000000))
docker stop selenium-video-test
echo "Time to stop container 20260202: $(expr $(($(date +%s%N)/1000000)) - $before) ms"
# docker logs selenium-video-test

# This has problems closing container, issue: https://github.com/SeleniumHQ/docker-selenium/issues/3093
sudo  \
NODE_IMAGE="selenium/standalone-chrome:145.0-20260222" VIDEO_IMAGE="selenium/video:ffmpeg-8.0-20260222" \
  FOLDER=$PWD FILE=20260222 USER=root docker compose -f compose-test.yml up -d  --quiet-pull
sleep 5
before=$(($(date +%s%N)/1000000))
docker stop selenium-video-test
echo "Time to stop container 20260222: $(expr $(($(date +%s%N)/1000000)) - $before) ms"
# docker logs selenium-video-test

# This takes 10 seconds to stop container
sudo  \
NODE_IMAGE="selenium/standalone-chrome:147.0-20260303" VIDEO_IMAGE="selenium/video:ffmpeg-8.1-20260303" \
  FOLDER=$PWD FILE=20260303 USER=root docker compose -f compose-test.yml up -d  --quiet-pull
sleep 5
before=$(($(date +%s%N)/1000000))
docker stop selenium-video-test
echo "Time to stop container 20260303: $(expr $(($(date +%s%N)/1000000)) - $before) ms"
# docker logs selenium-video-test

# This takes 10 seconds to stop container
sudo  \
NODE_IMAGE="selenium/standalone-chrome:147.0-20260404" VIDEO_IMAGE="selenium/video:ffmpeg-8.1-20260404" \
  FOLDER=$PWD FILE=20260404 USER=root docker compose -f compose-test.yml up -d  --quiet-pull
sleep 5
before=$(($(date +%s%N)/1000000))
docker stop selenium-video-test
echo "Time to stop container 20260404: $(expr $(($(date +%s%N)/1000000)) - $before) ms"
# docker logs selenium-video-test
