#Usa la imagen base que ha de ser definida antes en el servidor
#FROM node:22.20-slim
FROM node:22-alpine

RUN apk add --no-cache docker-cli

WORKDIR /app

# A wildcard is used to ensure both package.json AND package-lock.json are copied
COPY app/package*.json ./
COPY app/*.js ./
RUN npm install

RUN mkdir data
#establece permisos para luego ejecutar bajo el usuario sin privilegios
RUN chown -R node:node ./
USER node

EXPOSE 4449
CMD [ "node", "server.js" ]
