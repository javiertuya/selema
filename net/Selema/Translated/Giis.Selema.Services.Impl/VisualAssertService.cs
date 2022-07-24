/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Portable;
using Giis.Selema.Services;
using Giis.Visualassert;
using Sharpen;

namespace Giis.Selema.Services.Impl
{
	/// <summary>Management of VisualAssert comparisons between long strings, managing the html diff files and links from the log</summary>
	public class VisualAssertService : IVisualAssertService
	{
		private VisualAssert va;

		private ISelemaLogger logger;

		public VisualAssertService()
		{
			this.va = new VisualAssert();
		}

		/// <summary>Configures this service, called on attaching the service to a SeleniumManager</summary>
		public virtual IVisualAssertService Configure(ISelemaLogger logger, bool local, string projectRoot, string reportSubdir)
		{
			this.logger = logger;
			this.va.SetUseLocalAbsolutePath(local).SetReportSubdir(FileUtil.GetPath(projectRoot, reportSubdir));
			return this;
		}

		/// <summary>General assertion and log writing, intended to be used from a proxy in the SeleniumManager class</summary>
		public virtual void AssertEquals(string expected, string actual, string message, IMediaContext context, string testName)
		{
			if (!expected.Equals(actual))
			{
				if (logger != null && context != null)
				{
					//before assert determines file name and create log
					string diffFile = context.GetDiffFileName(testName);
					string diffUrl = "<a href=\"" + diffFile + "\">" + diffFile + "</a>";
					logger.Warn("Visual Assert differences: " + diffUrl + (string.Empty.Equals(message) ? string.Empty : " - Message: " + message));
					va.AssertEquals(expected, actual, message, diffFile);
				}
				else
				{
					//si no hay contexto ejecuta directametnte el assert y que genere el archivo
					va.AssertEquals(expected, actual, message, string.Empty);
				}
			}
		}
	}
}
