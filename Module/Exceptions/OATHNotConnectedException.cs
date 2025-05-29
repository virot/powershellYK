/// <summary>
/// Exception thrown when attempting to perform OATH operations without an active YubiKey connection.
/// Indicates that the OATH applet is not accessible or not properly connected.
/// 
/// .EXAMPLE
/// try {
///     // Attempt OATH operation
/// } catch (OATHNotConnectedException ex) {
///     Write-Error "OATH applet not connected. Please connect a YubiKey first."
/// }
/// </summary>

// Imports
using System;                                // Base exception class
using System.Runtime.Serialization;          // Serialization support

namespace powershellYK.Exceptions
{
    // Custom exception for OATH connection failures
    public class OATHNotConnectedException : Exception
    {
        // Default constructor
        public OATHNotConnectedException()
        {
        }

        // Constructor with custom message
        public OATHNotConnectedException(string message) : base(message)
        {
        }

        // Constructor with message and inner exception
        public OATHNotConnectedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}