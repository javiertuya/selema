FROM node:22-alpine
#FROM node:22.20-slim

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

COPY ./package*.json ./
COPY ./*.js ./
RUN mkdir data && npm install

# To run under the standard node user
#RUN chown -R node:node ./
#USER node

EXPOSE 4449
CMD [ "node", "server.js" ]
