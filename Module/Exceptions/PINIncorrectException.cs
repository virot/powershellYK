using System;
using System.Runtime.Serialization;

namespace powershellYK.Exceptions
{
    public class PINIncorrectException : Exception
    {
        public int RetriesRemaining { get; }
        public PINIncorrectException()
        {
        }

        public PINIncorrectException(string message, int? retriesRemaining) : base(message)
        {
            RetriesRemaining = retriesRemaining ?? 0;
        }

        public PINIncorrectException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}