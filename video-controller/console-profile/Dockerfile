FROM selenium/standalone-chrome:157.0-20261101

#WORKDIR /home/seluser
COPY ./console-profile.zip /home/seluser/
RUN unzip /home/seluser/console-profile.zip -d /home/seluser/ && rm /home/seluser/console-profile.zip