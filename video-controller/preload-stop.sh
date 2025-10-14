#!/bin/bash

# Stops a preloaded selenium browser node and sidecar video recorder container identified by a label

if [[ -z "$1" ]]; then
  echo "Usage: preload-stop.sh [label]"
  exit 1
fi

LABEL=$1
NODE_PREFIX="selenium-node"
VIDEO_PREFIX="selenium-video"

echo "Stopping preloaded selenium containers identified as '$LABEL'"

set -x
docker stop $VIDEO_PREFIX-$LABEL > /dev/null 2>&1
docker rm $VIDEO_PREFIX-$LABEL > /dev/null 2>&1
docker stop $NODE_PREFIX-$LABEL > /dev/null 2>&1
docker rm $NODE_PREFIX-$LABEL > /dev/null 2>&1
