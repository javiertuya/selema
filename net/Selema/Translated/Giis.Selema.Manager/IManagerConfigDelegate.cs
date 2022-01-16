/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Sharpen;

namespace Giis.Selema.Manager
{
	/// <summary>A delegate used to provide additional configurations to the SeleniumManager</summary>
	public interface IManagerConfigDelegate
	{
		void Configure(SeleniumManager sm);
	}
}
