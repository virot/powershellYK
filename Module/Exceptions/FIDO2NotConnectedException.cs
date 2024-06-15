using System;
using System.Runtime.Serialization;

namespace powershellYK.Exceptions
{
    public class FIDO2NotConnectedException : Exception
    {
        public FIDO2NotConnectedException()
        {
        }

        public FIDO2NotConnectedException(string message) : base(message)
        {
        }

        public FIDO2NotConnectedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}