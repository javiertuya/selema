/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Sharpen;

namespace Giis.Selema.Services
{
	/// <summary>Write actions to the Selema log</summary>
	public interface ISelemaLogger
	{
		void Trace(string message);

		void Debug(string message);

		void Info(string message);

		void Warn(string message);

		void Error(string message);
	}
}
