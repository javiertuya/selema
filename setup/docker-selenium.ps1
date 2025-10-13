#To run in local. Folders relative to the project root
#In net, customize the assets location

# https://github.com/SeleniumHQ/docker-selenium

# Dynamic grid
docker stop selenium
docker rm selenium
docker run -d --name selenium --shm-size="2g" `
    -p 4444:4444  `
    -e SE_VNC_VIEW_ONLY=true `
    -e SE_VNC_NO_PASSWORD=true `
    -e SE_NODE_MAX_SESSIONS=5 `
    -v ${PWD}/setup/docker-selenium.toml:/opt/selenium/docker.toml `
    -v ${PWD}/java/target/grid-assets:/opt/selenium/assets `
    -v /var/run/docker.sock:/var/run/docker.sock `
    selenium/standalone-docker:4.35.0-20250909
#    -e SE_VIDEO_RECORD_STANDALONE=true `
#Ver la sesion
# http://localhost:4444/ui/#/sessions
#parece que no se necesitan puertos para ver la sesionn
#-p 7900:7900

# http://localhost:4444/ui/#/sessions

# Standalone + video recording
docker network create grid
docker stop selenium-chrome
docker rm selenium-chrome
docker stop selenium-video
docker rm selenium-video
docker run -d -p 4444:4444 -p 6900:5900 --net grid --name selenium-chrome --shm-size="2g" `
  -e SE_NODE_MAX_SESSIONS=8 `
  selenium/standalone-chrome:4.35.0-20250909
docker run -d --net grid --name selenium-video `
  -e SE_VIDEO_FILE_NAME=video.mp4 `
  -e DISPLAY_CONTAINER_NAME=selenium-chrome `
  -v ${PWD}/java/target/preload-local:/videos `
  selenium/video:ffmpeg-8.0-20250909
# stop recoder here, the video service will start an stop when needed
sleep 5
docker stop selenium-video

  -e SE_VIDEO_FILE_NAME_SUFFIX=true `
  -e SE_VIDEO_FILE_NAME=auto `

docker start selenium-video
#docker exec -it video sudo chmod 777 /videos

