FROM node:24-alpine
#FROM node:24-slim

# Install docker-cli (Alpine)
RUN apk add --no-cache docker-cli

# Install docker-cli (Debian)
#RUN apt-get update && apt-get install -y ca-certificates curl gnupg lsb-release && rm -rf /var/lib/apt/lists/*
#RUN curl -fsSL https://download.docker.com/linux/debian/gpg | gpg --dearmor -o /usr/share/keyrings/docker.gpg
#RUN echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker.gpg] \
#    https://download.docker.com/linux/debian $(lsb_release -cs) stable" \
#    > /etc/apt/sources.list.d/docker.list
#RUN apt-get update && apt-get install -y docker-ce-cli && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy the lockfile explicitly: if it is missing the build fails instead of silently resolving a different tree
COPY package.json package-lock.json ./
COPY ./*.js ./
RUN mkdir data && npm ci --omit=dev

# The user is not set here: it is set at run time from docker-compose-controller.yml
# (USER variable, root by default), because this service needs access to the mounted docker socket.
# To run under the standard node user, uncomment the two lines below and add to the compose service
# the gid of the host docker group, so that the non root user can still use the socket:
#   user: ${USER:-node}
#   group_add:
#     - ${DOCKER_GID:?}
#RUN chown -R node:node ./
#USER node

EXPOSE 4449
CMD [ "node", "server.js" ]
