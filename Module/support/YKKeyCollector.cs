/// <summary>
/// Handles key collection and authentication for YubiKey operations.
/// Manages PIN, password, and management key interactions for PIV, FIDO2, and OATH.
/// 
/// .EXAMPLE
/// $collector = New-Object powershellYK.YKKeyCollector
/// $keyEntryData = New-Object Yubico.YubiKey.KeyEntryData
/// $result = $collector.YKKeyCollectorDelegate($keyEntryData)
/// 
/// .EXAMPLE
/// # Handle PIN verification
/// $collector = New-Object powershellYK.YKKeyCollector
/// $keyEntryData = New-Object Yubico.YubiKey.KeyEntryData -Property @{
///     Request = [Yubico.YubiKey.KeyEntryRequest]::VerifyPivPin
///     IsRetry = $true
///     RetriesRemaining = 2
/// }
/// $result = $collector.YKKeyCollectorDelegate($keyEntryData)
/// </summary>

// Imports
using powershellYK.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yubico.YubiKey;

namespace powershellYK
{
    // Handles key collection and authentication for YubiKey operations
    public class YKKeyCollector
    {
        // Processes key entry requests and handles authentication
        public bool YKKeyCollectorDelegate(KeyEntryData keyEntryData)
        {
            // Validate input
            if (keyEntryData is null)
            {
                return false;
            }

            // Handle retry scenarios
            if (keyEntryData.IsRetry)
            {
                switch (keyEntryData.Request)
                {
                    default:
                        throw new Exception("Unknown request. (Update YKKeyCollector)");

                    case KeyEntryRequest.AuthenticatePivManagementKey:
                        throw new Exception("Incorrect Management Key.");

                    case KeyEntryRequest.VerifyOathPassword:
                        throw new Exception("Incorrect Password.");

                    case KeyEntryRequest.ChangeFido2Pin:
                        throw new Exception("Failed to change FIDO2 PIN.");

                    case KeyEntryRequest.VerifyFido2Pin:
                    case KeyEntryRequest.VerifyPivPin:
                        if (!(keyEntryData.RetriesRemaining is null))
                        {
                            if (keyEntryData.RetriesRemaining == 0)
                            {
                                throw new PINIncorrectException("Incorrect PIN, no retries remaining.", 0);
                            }
                            else
                            {
                                YubiKeyModule._pivPIN = null;
                                throw new PINIncorrectException($"Incorrect PIN, {keyEntryData.RetriesRemaining} retries remaining.", keyEntryData.RetriesRemaining);
                            }
                        }
                        return false;
                }
            }

            byte[] currentValue;
            //byte[] newValue = null;

            switch (keyEntryData.Request)
            {
                default:
                    return false;

                case KeyEntryRequest.Release:
                    break;

                case KeyEntryRequest.VerifyPivPin:
                    if (YubiKeyModule._pivPIN is null)
                    {
                        throw new PIVNotConnectedException("PIN is required before issuing command, use 'Connect-YubikeyPIV' to authenticate.");
                    }
                    currentValue = System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._pivPIN!))!);
                    keyEntryData.SubmitValue(currentValue);
                    break;

                case KeyEntryRequest.VerifyFido2Pin:
                    if (YubiKeyModule._fido2PIN is null)
                    {
                        throw new FIDO2NotConnectedException("PIN is required before issuing command, use 'Connect-YubikeyFIDO2' to authenticate.");
                    }
                    currentValue = System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._fido2PIN!))!);
                    keyEntryData.SubmitValue(currentValue);
                    break;

                case KeyEntryRequest.VerifyOathPassword:
                    if (YubiKeyModule._OATHPassword is null)
                    {
                        throw new OATHNotConnectedException("Password is required before issuing command, use 'Connect-YubikeyOATH -Password' to authenticate.");
                    }
                    keyEntryData.SubmitValue(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._OATHPassword!))!));
                    break;

                case KeyEntryRequest.AuthenticatePivManagementKey:
                    keyEntryData.SubmitValue(YubiKeyModule._pivManagementKey);
                    break;

                case KeyEntryRequest.SetOathPassword:
                    keyEntryData.SubmitValues(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._OATHPassword ?? new System.Security.SecureString()))!),
                        System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._OATHPasswordNew ?? new System.Security.SecureString()))!));
                    break;

                case KeyEntryRequest.ChangePivPin:
                    throw new NotImplementedException("Change PIV PIN is not yet implemented");

                case KeyEntryRequest.SetFido2Pin:
                    keyEntryData.SubmitValue(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._fido2PINNew!))!));
                    break;

                case KeyEntryRequest.ChangeFido2Pin:
                    keyEntryData.SubmitValues(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._fido2PIN!))!), System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._fido2PINNew!))!));
                    break;
            }

            return true;
        }
    }
}
