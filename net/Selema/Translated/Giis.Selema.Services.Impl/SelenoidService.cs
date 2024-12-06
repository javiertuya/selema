/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Giis.Portable.Util;
using Giis.Selema.Services;
using Sharpen;

namespace Giis.Selema.Services.Impl
{
	public class SelenoidService : IBrowserService
	{
		private bool recordVideo = false;

		private bool enableVnc = false;

		private IDictionary<string, object> specialCapabilities = new Dictionary<string, object>();

		//si true habilita el video recording
		//si true habilita VNC par ver las sesiones desde selenoid-ui
		/// <summary>Activates the video recording, provided that the Selenoid server is configured for video recording</summary>
		public virtual IBrowserService SetVideo()
		{
			this.recordVideo = true;
			return this;
		}

		/// <summary>ctivates the VNC capabilities to be able to watch the test execution in real time (e.g.</summary>
		/// <remarks>ctivates the VNC capabilities to be able to watch the test execution in real time (e.g. using selenoid-ui)</remarks>
		public virtual IBrowserService SetVnc()
		{
			enableVnc = true;
			return this;
		}

		/// <summary>Adds a special capability other than the predefined video and vnc</summary>
		public virtual IBrowserService SetCapability(string key, object value)
		{
			specialCapabilities[key] = value;
			return this;
		}

		/// <summary>Gets the video recorder service associated with this browser service</summary>
		public virtual IVideoService GetVideoRecorder()
		{
			return recordVideo ? new SelenoidVideoService() : null;
		}

		/// <summary>Gets the capabilities that the WebDriver must configure to integrate with this service</summary>
		public virtual IDictionary<string, object> GetSeleniumOptions(string sessionName)
		{
			Dictionary<string, object> opts = new Dictionary<string, object>();
			opts["name"] = sessionName;
			opts["enableVNC"] = enableVnc;
			JavaCs.PutAll(opts, specialCapabilities);
			return opts;
		}
	}
}
