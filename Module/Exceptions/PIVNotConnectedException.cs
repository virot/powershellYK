using System;
using System.Runtime.Serialization;

namespace powershellYK.Exceptions
{
    public class PIVNotConnectedException : Exception
    {
        public PIVNotConnectedException()
        {
        }

        public PIVNotConnectedException(string message) : base(message)
        {
        }

        public PIVNotConnectedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}