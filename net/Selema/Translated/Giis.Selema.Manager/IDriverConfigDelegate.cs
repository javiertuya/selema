/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using OpenQA.Selenium;
using Sharpen;

namespace Giis.Selema.Manager
{
	/// <summary>A delegate used to provide additional driver configurations just after its creation</summary>
	public interface IDriverConfigDelegate
	{
		void Configure(IWebDriver driver);
	}
}
