using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Manager
{
    public class SelemaException : Exception
    {
        private static readonly long serialVersionUID = -2848837670059731155;
        public SelemaException(string message) : base(message)
        {
        }

        public SelemaException(string message, Exception cause) : base(message, cause)
        {
        }

        public SelemaException(Logger log, string message, Exception cause) : base(message, cause)
        {
            log.Error(message, cause);
        }
    }
}