/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Sharpen;

namespace Giis.Selema.Services
{
	/// <summary>Manages the filenames of media files produced durint testing (screenshots, videos, diff files)</summary>
	public interface IMediaContext
	{
		string GetReportFolder();

		string GetScreenshotFileName(string testName);

		string GetVideoFileName(string testName);

		string GetDiffFileName(string testName);
	}
}
