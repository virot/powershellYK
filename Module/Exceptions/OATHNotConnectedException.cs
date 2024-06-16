using System;
using System.Runtime.Serialization;

namespace powershellYK.Exceptions
{
    public class OATHNotConnectedException : Exception
    {
        public OATHNotConnectedException()
        {
        }

        public OATHNotConnectedException(string message) : base(message)
        {
        }

        public OATHNotConnectedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}