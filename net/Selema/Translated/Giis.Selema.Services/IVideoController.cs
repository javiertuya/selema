using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services
{
    /// <summary>
    /// Manages the start/stop of the video recording for preloaded browser containers
    /// </summary>
    public interface IVideoController
    {
        void Start();
        void Stop(string videoFileName);
    }
}