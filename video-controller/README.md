# Contents of the video-controller folder

- `app` the nodejs video controller server (js files) and its `Dockerfile` (node alpine image with the docker client)
- `docker-compose-controller.yml`: Parametrized compose file to build and run the video controller server in `app`
- `docker-compose-preload.yml`: Parametrized compose file to run a browser node and a sidecar video recorder for development and testing
- `docker-compose-grid.yml`: Parametrized compose file to run a selenium dynamic grid, with its configuration embedded
- `selenium-images.env`: The docker-selenium images compatible with Selema, passed to the above compose files with `docker compose --env-file`
- `vcmock`: A mock video recorder for integration testing (tests under the `video` subpackage of java and net projects)
- `setup-for-local-development.sh`: Scripts to build the required containers for development and testing
- `wait-container-ready.sh`: Waits until a container logs a given message (used in CI to wait for the grid and the video controller)
- `check-video-index.sh`: Checks that the videos listed in the selema video index exist and are complete MP4 files (used in CI after the tests)
- `smoke-test.sh`: Smoke test of the image, run by the `publish-video-controller` workflow before publishing it to GHCR

The CI workflow runs the video controller as a container built from `app/Dockerfile` in the `preload-remote` and `vcmock-remote` modes.
Running the server without a container (`npm start`) is only an alternative for debugging.
