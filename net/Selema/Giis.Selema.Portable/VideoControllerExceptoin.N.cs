using System;

namespace Giis.Selema.Portable
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