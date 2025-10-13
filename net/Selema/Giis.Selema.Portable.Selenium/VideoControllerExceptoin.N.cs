using System;
using System.Collections.Generic;
using System.Text;

namespace Giis.Selema.Portable.Selenium
{
    public class VideoControllerException : Exception
    {
        //private static readonly long serialVersionUID = -2848837670059731155;
        public VideoControllerException(string message) : base(message)
        {
        }

        public VideoControllerException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}