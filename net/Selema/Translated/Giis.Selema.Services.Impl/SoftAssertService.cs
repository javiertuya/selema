/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Portable.Util;
using Giis.Selema.Services;
using Giis.Visualassert;
using Sharpen;

namespace Giis.Selema.Services.Impl
{
	/// <summary>Management of SoftVisualAssert comparisons between long strings, managing the html diff files and links from the log</summary>
	public class SoftAssertService : ISoftAssertService
	{
		private SoftVisualAssert sva;

		private ISelemaLogger logger;

		public SoftAssertService()
		{
			this.sva = new SoftVisualAssert();
		}

		/// <summary>Configures this service, called on attaching the service to a SeleManager</summary>
		public virtual ISoftAssertService Configure(ISelemaLogger logger, bool local, string projectRoot, string reportSubdir)
		{
			this.logger = logger;
			this.sva.SetUseLocalAbsolutePath(local).SetReportSubdir(FileUtil.GetPath(projectRoot, reportSubdir));
			return this;
		}

		/// <summary>General soft assertion and log writing, intended to be used from a proxy in the SeleManager class</summary>
		public virtual void AssertEquals(string expected, string actual, string message, IMediaContext context, string testName)
		{
			if (!expected.Equals(actual))
			{
				if (logger != null && context != null)
				{
					//before assert determines file name and create log
					string diffFile = context.GetDiffFileName(testName);
					string diffUrl = "<a href=\"" + diffFile + "\">" + diffFile + "</a>";
					logger.Warn("Soft Visual Assert differences (Failure " + (sva.GetFailureCount() + 1) + "): " + diffUrl + (string.Empty.Equals(message) ? string.Empty : " - Message: " + message));
					sva.AssertEquals(expected, actual, message, diffFile);
				}
				else
				{
					//si no hay contexto ejecuta directametnte el assert y que genere el archivo
					sva.AssertEquals(expected, actual, message, string.Empty);
				}
			}
		}

		/// <summary>Throws and exception if at least one assertion failed including all assertion messages</summary>
		public virtual void AssertAll()
		{
			sva.AssertAll();
		}

		/// <summary>Resets the current failure messages that are stored</summary>
		public virtual void AssertClear()
		{
			sva.AssertClear();
		}
	}
}
