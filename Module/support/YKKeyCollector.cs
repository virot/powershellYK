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
                        return false;

                    case KeyEntryRequest.AuthenticatePivManagementKey:
                        throw new Exception("Incorrect Management Key.");
                        return false;

                    case KeyEntryRequest.VerifyPivPin:
                        if (!(keyEntryData.RetriesRemaining is null))
                        {
                            if (keyEntryData.RetriesRemaining == 0)
                            {
                                throw new Exception("Incorrect PIN, no retries remaining.");
                            }
                            else
                            {
                                throw new Exception($"Incorrect PIN, {keyEntryData.RetriesRemaining} retries remaining.");
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
                        throw new Exception("PIN not set, use Connect-YubikeyPIV -PIN XXXXXX");
                    }
                    currentValue = System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._pivPIN!))!);
                    keyEntryData.SubmitValue(currentValue);
                    break;

                case KeyEntryRequest.AuthenticatePivManagementKey:
                    keyEntryData.SubmitValue(YubiKeyModule._pivManagementKey);
                    break;
            }

            return true;
        }
    }
}
