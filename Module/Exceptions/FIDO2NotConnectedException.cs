/// <summary>
/// Exception thrown when attempting to perform FIDO2 operations without an active YubiKey connection.
/// Indicates that the FIDO2 applet is not accessible or not properly connected.
/// 
/// .EXAMPLE
/// try {
///     // Attempt FIDO2 operation
/// } catch (FIDO2NotConnectedException ex) {
///     Write-Error "FIDO2 applet not connected. Please connect a YubiKey first."
/// }
/// </summary>

// Imports
using System;                                // Base exception class
using System.Runtime.Serialization;          // Serialization support

namespace powershellYK.Exceptions
{
    // Custom exception for FIDO2 connection failures
    public class FIDO2NotConnectedException : Exception
    {
        // Default constructor
        public FIDO2NotConnectedException()
        {
        }

        // Constructor with custom message
        public FIDO2NotConnectedException(string message) : base(message)
        {
        }

        // Constructor with message and inner exception
        public FIDO2NotConnectedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}