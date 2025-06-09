/// <summary>
/// Exception thrown when attempting to perform PIV operations without an active YubiKey connection.
/// Indicates that the PIV applet is not accessible or not properly connected.
/// 
/// .EXAMPLE
/// try {
///     // Attempt PIV operation
/// } catch (PIVNotConnectedException ex) {
///     Write-Error "PIV applet not connected. Please connect a YubiKey first."
/// }
/// </summary>

// Imports
using System;                                // Base exception class
using System.Runtime.Serialization;          // Serialization support

namespace powershellYK.Exceptions
{
    // Custom exception for PIV connection failures
    public class PIVNotConnectedException : Exception
    {
        // Default constructor
        public PIVNotConnectedException()
        {
        }

        // Constructor with custom message
        public PIVNotConnectedException(string message) : base(message)
        {
        }

        // Constructor with message and inner exception
        public PIVNotConnectedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}