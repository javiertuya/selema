FROM selenium/standalone-chrome:144.0-20260202

#WORKDIR /home/seluser
COPY ./console-profile.zip /home/seluser/
RUN unzip /home/seluser/console-profile.zip -d /home/seluser/ && rm /home/seluser/console-profile.zip