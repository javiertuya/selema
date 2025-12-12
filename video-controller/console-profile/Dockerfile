FROM selenium/standalone-chrome:139.0-20251025

#WORKDIR /home/seluser
COPY ./console-profile.zip /home/seluser/
RUN unzip /home/seluser/console-profile.zip -d /home/seluser/ && rm /home/seluser/console-profile.zip