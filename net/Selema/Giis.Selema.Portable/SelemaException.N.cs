using NLog;
using System;

namespace Giis.Selema.Portable
{
	/// <summary>
	/// Generic custom exception for compatibility Java/C#
	/// </summary>
	public class SelemaException : Exception
	{
		public SelemaException(Exception e) : base(e.Message) { }

		public SelemaException(string message) : base(message) { }

		public SelemaException(string message, Exception cause) : base(message, cause) { }
		public SelemaException(Logger log, string message, Exception cause) : base(message, cause) {
			log.Error(cause, message);
		}
	}
}
