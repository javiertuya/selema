/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Selema.Portable;
using Sharpen;

namespace Giis.Selema.Manager
{
	/// <summary>Static configuration parameters at the instantiation of a SeleManager (location, name, etc)</summary>
	public class SelemaConfig
	{
		private const string DefaultName = "selema";

		private string projectRoot = Parameters.DefaultProjectRoot;

		private string reportSubdir = Parameters.DefaultReportSubdir;

		private string logName = DefaultName;

		//
		//Path to the project root (depends on the platform)
		//Path to the selema reports folder relative to projectRoot (depends on the platform)
		/// <summary>Gets the platform name (java or net), useful if conditional execution of tests is needed</summary>
		public virtual string GetPlatformName()
		{
			return Parameters.PlatformName;
		}

		/// <summary>True if running in java platform</summary>
		public virtual bool IsJava()
		{
			return "java".Equals(GetPlatformName());
		}

		/// <summary>True if running in net platform</summary>
		public virtual bool IsNet()
		{
			return "net".Equals(GetPlatformName());
		}

		/// <summary>Gets the project root currently configured</summary>
		public virtual string GetProjectRoot()
		{
			return projectRoot;
		}

		/// <summary>
		/// Changes the location of the project root;
		/// Default is current folder on Java and four levels up on NET
		/// (this is the solution folder provided that the test project is located just below the solution folder)
		/// </summary>
		public virtual SelemaConfig SetProjectRoot(string root)
		{
			projectRoot = root;
			return this;
		}

		/// <summary>Gets the selema report folder currently configured</summary>
		public virtual string GetReportSubdir()
		{
			return reportSubdir;
		}

		/// <summary>Changes the name of the report folder (relative to the project root).</summary>
		/// <remarks>Changes the name of the report folder (relative to the project root). Default is `target` (on Java) and `reports` (on .NET).</remarks>
		public virtual SelemaConfig SetReportSubdir(string subdir)
		{
			reportSubdir = subdir;
			return this;
		}

		public virtual string GetReportDir()
		{
			return FileUtil.GetPath(projectRoot, reportSubdir);
		}

		public virtual string GetName()
		{
			return logName;
		}

		/// <summary>Sets the name of the SeleManager (default is selema), useful when logs from different instances must be kept separated</summary>
		public virtual SelemaConfig SetName(string name)
		{
			logName = name;
			return this;
		}

		public virtual string GetQualifier()
		{
			return logName.Equals(DefaultName) ? string.Empty : "-" + logName;
		}

		public virtual string GetLogName()
		{
			return "selema-log" + GetQualifier() + ".html";
		}
	}
}
