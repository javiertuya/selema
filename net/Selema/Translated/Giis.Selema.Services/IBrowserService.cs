/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Sharpen;

namespace Giis.Selema.Services
{
	public interface IBrowserService
	{
		/// <summary>Activates the video recording, provided that the service is configured for video recording</summary>
		IBrowserService SetVideo();

		/// <summary>Activates the VNC capabilities to be able to watch the test execution in real time</summary>
		IBrowserService SetVnc();

		/// <summary>Adds a special capability other than the predefined video and vnc</summary>
		IBrowserService SetCapability(string key, object value);

		/// <summary>Gets the video recorder service associated with this browser service</summary>
		IVideoService GetVideoRecorder();

		/// <summary>Gets the capabilities that the WebDriver must configure to integrate with this service</summary>
		IDictionary<string, object> GetSeleniumOptions(string sessionName);
	}
}
