FROM node:24-alpine@sha256:116b800c3754b440ee0aa3ed2f20f5bf3d99a554bfd9adeea09ea8017f6e85f5

# Docker client to manage the recorder containers through the mounted docker socket.
# The static binary is copied from the official image, so that it does not depend on the packages of the base image
# (e.g. the docker.io package of Debian bookworm is too old for current docker daemons).
COPY --from=docker:29-cli@sha256:34507eb5069d8be82c95dd3b6987d4250fd5fee49b82e2509b7d7a7030740d6d /usr/local/bin/docker /usr/local/bin/docker

WORKDIR /app

# Copy the lockfile explicitly: if it is missing the build fails instead of silently resolving a different tree
COPY package.json package-lock.json ./
COPY ./*.js ./
RUN mkdir data && npm ci --omit=dev

# The user is not set here: it is set at run time from docker-compose-controller.yml
# (CONTAINER_USER variable, root by default), because this service needs access to the mounted docker socket.
# To run under the standard node user, uncomment the two lines below and add to the compose service
# the gid of the host docker group, so that the non root user can still use the socket:
#   user: ${CONTAINER_USER:-node}
#   group_add:
#     - ${DOCKER_GID:?}
#RUN chown -R node:node ./
#USER node

EXPOSE 4449
CMD [ "node", "server.js" ]
