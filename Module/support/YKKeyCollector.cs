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

    public class YKKeyCollector
    {

        public bool YKKeyCollectorDelegate(KeyEntryData keyEntryData)
        {
            if (keyEntryData is null)
            {
                return false;
            }

            if (keyEntryData.IsRetry)
            {
                switch (keyEntryData.Request)
                {
                    default:
                        throw new Exception("Unknown request.");
                        //return false;

                    case KeyEntryRequest.AuthenticatePivManagementKey:
                        throw new Exception("Incorrect Management Key.");
                    //return false;

                    case KeyEntryRequest.VerifyOathPassword:
                        throw new Exception("Incorrect Password.");
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
                        throw new PIVNotConnectedException("Needed PIN not set, use Connect-YubikeyPIV to authorize");
                    }
                    currentValue = System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._pivPIN!))!);
                    keyEntryData.SubmitValue(currentValue);
                    break;

                case KeyEntryRequest.VerifyFido2Pin:
                    if (YubiKeyModule._fido2PIN is null)
                    {
                        throw new FIDO2NotConnectedException("Needed PIN not set, use Connect-YubikeyFIDO2 to authorize");
                    }
                    currentValue = System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._fido2PIN!))!);
                    keyEntryData.SubmitValue(currentValue);
                    break;

                case KeyEntryRequest.VerifyOathPassword:
                    if (YubiKeyModule._OATHPassword is null)
                    {
                        throw new OATHNotConnectedException("Needed password not set, use Connect-OATH to authorize");
                    }
                    keyEntryData.SubmitValue(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._OATHPassword!))!));
                    break;

                case KeyEntryRequest.AuthenticatePivManagementKey:
                    keyEntryData.SubmitValue(YubiKeyModule._pivManagementKey);
                    break;
            }

            return true;
        }
    }
}
