#!/usr/bin/env bash
# Smoke test of the video controller image (SMOKE_IMAGE), run by the publication workflow before the push.
# Runs the controller as in production (docker socket and videos folder mounted) and checks that:
# - stopping a recording reaches the docker daemon through the docker client of the image:
#   the recorder container (a stopped dummy one) is stopped, and the video is not found (404)
# - deleting a video removes it from the mounted folder (200)
set -u

videos=$(mktemp -d)
docker create --name selenium-video-smoke "$SMOKE_IMAGE" true >/dev/null
docker run -d --name smoke -p 4449:4449 \
  -v "$videos":/app/videos -v /var/run/docker.sock:/var/run/docker.sock "$SMOKE_IMAGE"
trap 'echo "--- container logs:"; docker logs smoke' EXIT
for i in $(seq 1 30); do
  docker logs smoke 2>&1 | grep -q "Listening on port" && break
  [ "$i" -eq 30 ] && { echo "::error::The service did not start in 30 seconds"; exit 1; }
  sleep 1
done

url=http://localhost:4449/selema-video-controller/recording/smoke
status=$(curl -sS -o stop.json -w '%{http_code}' "$url")
echo "stop: $status $(cat stop.json)"
[ "$status" = "404" ] && grep -q 'Video file not found after recording' stop.json \
  || { echo "::error::Unexpected stop response"; exit 1; }
echo "dummy video" > "$videos/smoke.mp4"
status=$(curl -sS -o delete.json -w '%{http_code}' -X DELETE "$url")
echo "delete: $status $(cat delete.json)"
[ "$status" = "200" ] && [ ! -f "$videos/smoke.mp4" ] \
  || { echo "::error::Unexpected delete response, or the video was not deleted"; exit 1; }
