FROM ubuntu:noble
#FROM debian:bullseye-slim

WORKDIR /app

# Instala bash y coreutils para scripts más completos
#RUN apk add --no-cache bash coreutils

COPY entrypoint.sh shutdown.sh sample.mp4 ./
RUN mkdir videos && chmod +x entrypoint.sh shutdown.sh

# Copia el video al arranque
ENTRYPOINT ["./entrypoint.sh"]