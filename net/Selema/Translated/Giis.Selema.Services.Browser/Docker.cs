using Giis.Portable.Util;
using Giis.Selema.Portable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services.Browser
{
    public class Docker
    {
        private Docker()
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
        /// Waits until a line of the container log written since the container was started contains the expected strings.
        /// The whole log since the start is checked (not only the last line) because other processes in the container
        /// may write after the expected line (e.g. the uploader of the docker-selenium video recorder images).
        /// The start time is taken from docker to exclude the log of previous runs of the same container.
        /// </summary>
        public static void WaitDocker(string container, string expectedLog1, string expectedLog2, int timeoutSeconds)
        {
            string startedAt = CommandLine.RunCommand("docker inspect -f '{{.State.StartedAt}}' " + container).Replace("'", "").Trim();

            // poll the docker log until expected string found
            string lastLine = "";
            for (int i = 0; i < timeoutSeconds * 10; i++)
            {
                string[] lines = JavaCs.SplitByChar(CommandLine.RunCommand("docker logs --since " + startedAt + " " + container), '\n');
                foreach (string line in lines)
                {
                    if (line.Contains(expectedLog1) && line.Contains(expectedLog2))
                        return;
                    if (!"".Equals(line.Trim()))
                        lastLine = line;
                }

                JavaCs.Sleep(100);
            }

            throw new VideoControllerException("Container did not become ready in time, last log message was: " + lastLine);
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