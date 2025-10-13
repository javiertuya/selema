#To run in local. Folders relative to the project root
#In net, customize the assets location

# https://github.com/SeleniumHQ/docker-selenium

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