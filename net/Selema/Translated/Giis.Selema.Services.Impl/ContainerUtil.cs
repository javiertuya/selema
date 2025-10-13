using Giis.Portable.Util;
using Giis.Selema.Portable.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services.Impl
{
    public class ContainerUtil
    {
        private ContainerUtil()
        {
            throw new InvalidOperationException("Utility class");
        }

        /// <summary>
        /// Runs a docker command (start or stop), fails if does not respond with the created container name.
        /// </summary>
        public static void RunDocker(string verb, string container)
        {
            string command = "docker " + verb + " " + container;
            string dockerOut = CommandLine.RunCommand(command);
            if (!container.Equals(dockerOut.Trim()))
                throw new VideoControllerException(command + " failed. " + dockerOut);
        }

        /// <summary>
        /// Runs a docker command (start or stop) and waits until the container log contains the expected strings.
        /// </summary>
        public static void WaitDocker(string container, string expectedLog1, string expectedLog2, int timeoutSeconds)
        {

            // poll the docker log until expected string found
            string logOut = "";
            for (int i = 0; i < timeoutSeconds * 10; i++)
            {
                logOut = CommandLine.RunCommand("docker logs --tail 1 " + container);
                if (logOut.Contains(expectedLog1) && logOut.Contains(expectedLog2))
                    return;
                JavaCs.Sleep(100);
            }

            throw new VideoControllerException("Container did not become ready in time, last log message was: " + logOut);
        }

        /// <summary>
        /// Returns the container status (exited, running or paused), error message if it does not exist
        /// </summary>
        public static string GetContainerStatus(string name)
        {
            return CommandLine.RunCommand("docker inspect -f '{{.State.Status}}' " + name).Replace("'", "").Trim();
        }
    }
}