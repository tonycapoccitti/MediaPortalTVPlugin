using System;
using System.Runtime.Serialization;

namespace MediaBrowser.Plugins.MediaPortal.Services.Exceptions
{
    [Serializable]
    public class ServiceAuthenticationException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ServiceAuthenticationException()
        {
        }

        public ServiceAuthenticationException(string message)
            : base(message)
        {
        }

        public ServiceAuthenticationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ServiceAuthenticationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
