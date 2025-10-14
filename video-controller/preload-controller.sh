#!/bin/bash

# Start a video controller container to manage video recording for selenium nodes with a sidecar video recorder

if [[ -z "$1" || -z "$2" || -z "$3" ]]; then
  ECHO "Usage: preload-controller.sh [context] [network] [folder] [user (optional)] [ports (optional)] [connection (optional)]"
  echo "  context: the docker context to build the video controller image from (path to folder with Dockerfile)"
  echo "  network: the name of the docker network to connect the container to" 
  echo "  folder: the folder to save the recorded videos to (will be created if it does not exist)"
  echo "  ports: port mappings for the video controller container (optional, format: [ip:]hostPort:containerPort)"
  echo "  user: the user to run the video controller container as (optional, default is root, format: uid[:gid])" 
  echo "  connection: connection mode for the docker api, either 'socket' or 'tcp' (optional, default is 'socket')"
  echo "    'socket' for volume mount the linux docker socket /var/run/docker.sock"
  echo "    'tcp' for tcp access to the docker api (from windows, set in docker desktop settings to expose the api on tcp://localhost:2375 without TLS)"
  exit 1
fi

CONTAINER_NAME="selema-video-controller"
IMAGE_NAME="selema-video-controller"

CONTEXT=$1
NETWORK=$2
FOLDER=$3
PORTS=$4
USER=$5
CONNECTION=$6

PORTS_PARAM=""
if [[ -n "$PORTS" ]]; then
  PORTS_PARAM="-p $PORTS"
fi
if [[ -z "$USER" ]]; then
  USER="root"
fi
USER_PARAM="--user $USER"

if [[ -z "$CONNECTION" || "$CONNECTION" == "socket" ]]; then
  SOCKET_PARAM="-v /var/run/docker.sock:/var/run/docker.sock"
elif [[ "$CONNECTION" == "tcp" ]]; then
  SOCKET_PARAM="-e DOCKER_HOST=tcp://host.docker.internal:2375"
else
  echo "CONNECTION parameter must be 'socket' or 'tcp'"
  echo "  'socket' for volume mount the linux docker socket /var/run/docker.sock"
  echo "  'tcp' for tcp access to the docker api (from windows, set in docker desktop settings to expose the api on tcp://localhost:2375 without TLS)"
  exit 1
fi

echo "Starting video controller container '$CONTAINER_NAME' on docker network '$NETWORK' port export '$PORTS' with user '$USER'"
echo "Building image from docker context '$CONTEXT'"
echo "Sharing videos in folder '$FOLDER'"

set -x
docker build -t $IMAGE_NAME $CONTEXT
docker stop $CONTAINER_NAME > /dev/null 2>&1
docker rm $CONTAINER_NAME > /dev/null 2>&1

docker run -d --name $CONTAINER_NAME --net $NETWORK $PORTS_PARAM $USER_PARAM --restart unless-stopped \
    -v $FOLDER:/app/videos \
    $SOCKET_PARAM \
    $IMAGE_NAME
