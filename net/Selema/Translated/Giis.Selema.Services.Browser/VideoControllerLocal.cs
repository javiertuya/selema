using NLog;
using Giis.Portable.Util;
using Giis.Selema.Portable;
using Giis.Selema.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services.Browser
{
    /// <summary>
    /// Video controller to use when both the recorder container and the tests run in the same VM.
    /// 
    /// Requires the container running the test have access to docker and to the folders when the videos are stored. Any
    /// unexpected behaviour raises an exception that must be handled by the caller video service.
    /// </summary>
    public class VideoControllerLocal : IVideoController
    {
        readonly Logger log = LogManager.GetCurrentClassLogger();
        private string videoContainer;
        private string sourceFile;
        private string targetFolder;
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="label">to uniquely identify the video recorder container</param>
        /// <param name="sourceFile">name of the recorded video (with path)</param>
        /// <param name="targetFolder">where the recorded videos will be stored after recording</param>
        public VideoControllerLocal(string label, string sourceFile, string targetFolder)
        {
            this.videoContainer = label;
            this.sourceFile = sourceFile;
            this.targetFolder = targetFolder;
        }

        public virtual void Start()
        {

            // First clean the video file (if exists) to prevent the recorder concatenating videos
            CommandLine.FileDelete(sourceFile, false);

            // The recorder should be created and stopped in order to start and record video now.
            // If not, stop now
            if (!"exited".Equals(Docker.GetContainerStatus(videoContainer)))
            {
                log.Debug("Video recorder " + videoContainer + " is not stopped, restarting");
                Docker.RunDocker("stop", videoContainer);
                Docker.WaitDocker(videoContainer, "Shutdown complete", "", 5);
            }

            log.Debug("Starting video recorder: " + videoContainer);
            Docker.RunDocker("start", videoContainer);
            Docker.WaitDocker(videoContainer, "Display", "is open", 5);
        }

        public virtual void Stop(string videoName)
        {
            log.Debug("Stopping video recorder: " + videoContainer);
            Docker.RunDocker("stop", videoContainer);
            Docker.WaitDocker(videoContainer, "Shutdown complete", "", 5);

            // copy the video file to its destination and then remove, this should not fail
            if (!CommandLine.FileExists(sourceFile))
                throw new VideoControllerException("Video file not found after recording: " + sourceFile);
            log.Debug("Saving recorded video file to: " + videoName);
            CommandLine.FileCopy(sourceFile, FileUtil.GetPath(targetFolder, videoName));
            CommandLine.FileDelete(sourceFile, true);
        }
    }
}