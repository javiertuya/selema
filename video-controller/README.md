# Contents of the video-controller folder

- `app` the nodejs video controller server (js files)
- `docker-compose-controller.yml`: Parametrized compose file to build and run the video controller server in `app`
- `docker-compose-preload.yml`: Parametrized compose file to the a browser node and a sidecar video recorder for development and testing
- `vcmock`: A mock video recorder for integration testing (tests under the `video` subpackage of java and net projects)
- `setup-for-local-development.sh`: Scripts to build the required containers for development and testing
