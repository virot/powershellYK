/// <summary>
/// Exception thrown when an incorrect PIN is provided for YubiKey operations.
/// Tracks the number of PIN retries remaining before the YubiKey is blocked.
/// 
/// .EXAMPLE
/// try {
///     // Attempt YubiKey operation with PIN
/// } catch (PINIncorrectException ex) {
///     Write-Warning "Incorrect PIN. Retries remaining: $($ex.RetriesRemaining)"
/// }
/// </summary>

// Imports
using System;                                // Base exception class
using System.Runtime.Serialization;          // Serialization support

namespace powershellYK.Exceptions
{
    // Custom exception for PIN validation failures
    public class PINIncorrectException : Exception
    {
        // Number of PIN retries remaining before YubiKey is blocked
        public int RetriesRemaining { get; }

        // Default constructor
        public PINIncorrectException()
        {
        }

        // Constructor with message and retry count
        public PINIncorrectException(string message, int? retriesRemaining) : base(message)
        {
            RetriesRemaining = retriesRemaining ?? 0;
        }

        // Constructor with message and inner exception
        public PINIncorrectException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}