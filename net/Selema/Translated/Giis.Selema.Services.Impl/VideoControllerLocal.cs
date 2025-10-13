using NLog;
using Giis.Portable.Util;
using Giis.Selema.Manager;
using Giis.Selema.Portable.Selenium;
using Giis.Selema.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services.Impl
{
    /// <summary>
    /// Video controller to use when both the recorder container and the tests run in the same VM. Requires the container
    /// running the test have access to docker and to the folders when the videos are stored. Any unexpected behaviour raises
    /// an exception that must be handled by the caller video service.
    /// </summary>
    public class VideoControllerLocal : IVideoController
    {
        readonly Logger log = LogManager.GetCurrentClassLogger();
        private string videoContainer;
        private string sourceFile;
        private string targetFolder;
        public VideoControllerLocal(string videoContainer, string sourceFile, string targetFolder)
        {
            this.videoContainer = videoContainer;
            this.sourceFile = sourceFile;
            this.targetFolder = targetFolder;
        }

        public virtual void Start()
        {

            // First clean the video file (if exists) to prevent the recorder concatenating videos
            CommandLine.FileDelete(sourceFile, false);
            log.Debug("Starting video container: " + videoContainer);
            RunDockerWait("start", videoContainer, "Display selenium-chrome:99.0 is open", 5);
        }

        public virtual void Stop(string videoName)
        {
            log.Debug("Stopping video container: " + videoContainer);
            RunDockerWait("stop", videoContainer, "Shutdown complete", 5);

            // copy the video file to its destination and then remove, this should not fail
            log.Debug("Saving recorded video file to: " + videoName);
            CommandLine.FileCopy(sourceFile, FileUtil.GetPath(targetFolder, videoName));
            CommandLine.FileDelete(sourceFile, true);
        }

        /// <summary>
        /// Runs a docker command (start or stop) and waits until the container log contains the expected string.
        /// </summary>
        private void RunDockerWait(string verb, string container, string expectedLog, int timeoutSeconds)
        {
            string command = "docker " + verb + " " + container;
            string dockerOut = CommandLine.RunCommand(command);
            if (!container.Equals(dockerOut.Trim()))
                throw new SelemaException(command + " failed. " + dockerOut);

            // poll the docker log until expected string found
            string logOut = "";
            for (int i = 0; i < timeoutSeconds * 10; i++)
            {
                logOut = CommandLine.RunCommand("docker logs --tail 1 " + container);
                if (logOut.Contains(expectedLog))
                    return;
                JavaCs.Sleep(100);
            }

            throw new SelemaException(command + " failed. Last docker log does not contain the expected confirmation, but was: " + logOut);
        }
    }
}