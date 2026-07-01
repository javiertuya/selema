#!/bin/bash
#Sample approach to wait until a container is ready
#by looking for a string in container log
container=$1
target=$2

attempt=0
while [ $attempt -le 120 ]; do
    attempt=$(( $attempt + 1 ))
    echo "Waiting for container $container ready (attempt: $attempt)..."
    result=$(docker logs $container)
    if grep -q "$target" <<< $result ; then
      echo "Container $container is ready!"
      exit 0
    fi
    sleep 1
done
echo "ERROR: Container $container is not ready after maximum number of attempts"
exit 1