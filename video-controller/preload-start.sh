#!/bin/bash

# Starts a preloaded selenium browser node and sidecar video recorder container
# The containers can be identified by a label, which is part of the container names
# The video recorder saves the recorded video to a specified folder with the name <label>.mp4

if [[ -z "$1" || -z "$2" || -z "$3" ]]; then
  echo "Usage: preload-start.sh [label] [network] [folder] [user (optional)] [ports (optional)]"
  echo "  label: identifier for the selenium node and video recorder containers, the name of the containers will be <prefix>-<label>"
  echo "  network: the name of the docker network to connect the containers to"
  echo "  folder: the folder to save the recorded videos to (will be created if it does not exist)"
  echo "  ports: port mappings for the browser node container (optional, format: [ip:]hostPort:containerPort)"
  echo "  user: the user to run the video recorder container as (optional), default is root, format: uid[:gid])"
  exit 1
fi

LABEL=$1
NETWORK=$2
FOLDER=$3
PORTS=$4
USER=$5
NODE_PREFIX="selenium-node"
VIDEO_PREFIX="selenium-video"
NODE_IMAGE="selenium/standalone-chrome:4.35.0-20250909"
VIDEO_IMAGE="selenium/video:ffmpeg-8.0-20250909"

PORTS_PARAM=""
if [[ -n "$PORTS" ]]; then
  PORTS_PARAM="-p $PORTS"
fi
if [[ -z "$USER" ]]; then
  USER="root"
fi
USER_PARAM="--user $USER"

echo "Starting preloaded selenium containers identified as '$LABEL' on docker network '$NETWORK' port export '$PORTS' with user '$USER'"
echo "Composed of a selenium browser node '$NODE_PREFIX-$LABEL' and a sidecar video recorder '$VIDEO_PREFIX-$LABEL'"
echo "Saving videos as '$LABEL.mp4' to folder '$FOLDER'"

set -x
docker stop $VIDEO_PREFIX-$LABEL > /dev/null 2>&1
docker rm $VIDEO_PREFIX-$LABEL > /dev/null 2>&1 
docker stop $NODE_PREFIX-$LABEL > /dev/null 2>&1
docker rm $NODE_PREFIX-$LABEL > /dev/null 2>&1
mkdir -p $FOLDER
docker run -d --name $NODE_PREFIX-$LABEL --net $NETWORK $PORTS_PARAM \
  -e SE_NODE_MAX_SESSIONS=8 --shm-size="2g" \
  $NODE_IMAGE
docker run -d --name $VIDEO_PREFIX-$LABEL --net $NETWORK $USER_PARAM \
  -e SE_VIDEO_FILE_NAME=$LABEL.mp4 \
  -e DISPLAY_CONTAINER_NAME=$NODE_PREFIX-$LABEL \
  -v $FOLDER:/videos \
  $VIDEO_IMAGE
sleep 2
docker stop $VIDEO_PREFIX-$LABEL
