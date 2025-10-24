using NUnit.Framework;
using NLog;
using Giis.Portable.Util;
using Giis.Selema.Services;
using Giis.Selema.Services.Browser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Video
{
    /// <summary>
    /// Partially integrated test of a Video Controller (remote) that calls the video-controller server
    /// 
    /// Tests inherit from the local video controller tests and share the same mock recorder container. The only difference
    /// is where the mock recorded video is stored (under a folder in the video-controller server).
    /// 
    /// This requires a previous setting to launch the server (with or without a container).
    /// See commands at video-controller/setup-controller-local-dev.sh
    /// </summary>
    public class TestVideoControllerRemote : TestVideoControllerLocal
    {
        static readonly Logger log = LogManager.GetCurrentClassLogger(); //TestVideoControllerRemote));
        protected static readonly string CONTROLLER_URL = "http://localhost:4449/selema-video-controller";
        protected override void FileSystemSetup()
        {

            // In remote, the mapped folder is inside the remote controller app (with or withot container)
            mappedFolder = FileUtil.GetPath(ROOT, "../video-controller/app/videos");
            recordedVideo = mappedFolder + "/mock.mp4";
            targetFolder = FileUtil.GetPath(ROOT, "..", "video-controller/target/vcmock-target");
        }

        protected override IVideoController GetController()
        {
            return new VideoControllerRemote("mock", CONTROLLER_URL, targetFolder);
        }

        [Test]
        public override void TestPassRegularLifeCycle()
        {
            base.TestPassRegularLifeCycle();
        }

        [Test]
        public override void TestPassWhenPreviousRunDidNotDeleteVideo()
        {
            base.TestPassWhenPreviousRunDidNotDeleteVideo();
        }

        [Test]
        public override void TestPassWhenPreviousRunDidNotStopRecorder()
        {
            base.TestPassWhenPreviousRunDidNotStopRecorder();
        }

        [Test]
        public override void TestFailOnStartBecauseRecorderDoesNotExist()
        {
            base.TestFailOnStartBecauseRecorderDoesNotExist();
        }

        [Test]
        public override void TestFailOnStopBecauseCanNotCopyVideo()
        {
            base.TestFailOnStopBecauseCanNotCopyVideo();
        }

        [Test]
        public override void TestFailOnStopBecauseCanNotStopRecorder()
        {
            base.TestFailOnStopBecauseCanNotStopRecorder();
        }

        [Test]
        public override void TestContainerNotReadyAfterWaitForLogMessage()
        {
            base.TestContainerNotReadyAfterWaitForLogMessage();
        }
    }
}