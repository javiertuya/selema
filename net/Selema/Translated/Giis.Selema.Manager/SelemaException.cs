/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using NLog;
using Sharpen;

namespace Giis.Selema.Manager
{
	[System.Serializable]
	public class SelemaException : Exception
	{
		private const long serialVersionUID = -2848837670059731155L;

		public SelemaException(string message)
			: base(message)
		{
		}

		public SelemaException(string message, Exception cause)
			: base(message, cause)
		{
		}

		public SelemaException(Logger log, string message, Exception cause)
			: base(message, cause)
		{
			log.Error(message, cause);
		}
	}
}
