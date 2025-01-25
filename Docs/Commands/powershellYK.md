---
Module Name: powershellYK
Module Guid: d947dd9b-87eb-49ea-a373-b91c7acc0917
Download Help Link: {{ Update Download Link }}
Help Version: 0.0.12.1
Locale: en-US
---

# powershellYK Module
## Description
{{ Fill in the Description }}

## powershellYK Cmdlets
### [Assert-YubikeyPIV](Assert-YubikeyPIV.md)
Create attestation certificate

### [Block-YubikeyPIV](Block-YubikeyPIV.md)
Block out PIN or PUK codes

### [Build-YubiKeyPIVCertificateSigningRequest](Build-YubiKeyPIVCertificateSigningRequest.md)
Creates a CSR for a slot in the YubiKey.

### [Build-YubikeyPIVSignCertificate](Build-YubikeyPIVSignCertificate.md)
Sign a certificate request with a YubiKey.

### [Confirm-YubiKeyAttestion](Confirm-YubiKeyAttestion.md)
Confirm YubiKey Attestion.

### [Connect-Yubikey](Connect-Yubikey.md)
Connect the module to the YubiKey.

### [Connect-YubiKeyFIDO2](Connect-YubiKeyFIDO2.md)
Connect to the FIDO2 session.

### [Connect-YubikeyOATH](Connect-YubikeyOATH.md)
Connect to the OATH part of the connected YubiKey.

### [Connect-YubikeyPIV](Connect-YubikeyPIV.md)
Connect PIV module

### [ConvertTo-AltSecurity](ConvertTo-AltSecurity.md)
Generate the alt security security identities for a certificate

### [Disconnect-Yubikey](Disconnect-Yubikey.md)
Disconnects the YubiKey

### [Enable-powershellYKSDKLogging](Enable-powershellYKSDKLogging.md)
Enables logging from the Yubico SDK.

### [Enable-YubikeyFIDO2EnterpriseAttestation](Enable-YubikeyFIDO2EnterpriseAttestation.md)
Enables the Enterprise Attestion feature on the YubiKey FIDO2 device.

### [Export-YubikeyPIVCertificate](Export-YubikeyPIVCertificate.md)
Export certificate from YubiKey PIV

### [Find-Yubikey](Find-Yubikey.md)
Lists all YubiKeys on system

### [Get-powershellYKInfo](Get-powershellYKInfo.md)
Get module internal information.

### [Get-Yubikey](Get-Yubikey.md)
Returns the connected YubiKey

### [Get-YubiKeyBIOFingerprint](Get-YubiKeyBIOFingerprint.md)
List fingerprint templates registered on a YubiKey Bio or YubiKey Bio Multi-Protocol Edition (MPE).

### [Get-YubikeyFIDO2](Get-YubikeyFIDO2.md)
Get FIDO2 information from YubiKey

### [Get-YubikeyFIDO2Credential](Get-YubikeyFIDO2Credential.md)
Read the FIDO2 discoverable credentials

### [Get-YubikeyOATH](Get-YubikeyOATH.md)
Get information about the OATH module

### [Get-YubikeyOATHAccount](Get-YubikeyOATHAccount.md)
List all OATH accounts

### [Get-YubikeyOTP](Get-YubikeyOTP.md)
YubiKey OTP Information

### [Get-YubikeyPIV](Get-YubikeyPIV.md)
Gets information about the PIV module and specific slots.

### [Import-YubikeyPIV](Import-YubikeyPIV.md)
Import certificate

### [Lock-Yubikey](Lock-Yubikey.md)
Lock the YubiKey configuration

### [Move-YubikeyPIV](Move-YubikeyPIV.md)
Move a key from one slot to another

### [New-YubiKeyFIDO2Credential](New-YubiKeyFIDO2Credential.md)
Creates a new FIDO2 credential on the connected YubiKey.
For more complete examples see: https://github.com/virot/powershellYK/tree/master/Docs/Examples

### [New-YubikeyOATHAccount](New-YubikeyOATHAccount.md)
Created a TOTP or HOTP account

### [New-YubiKeyPIVKey](New-YubiKeyPIVKey.md)
Create a new private key

### [New-YubikeyPIVSelfSign](New-YubikeyPIVSelfSign.md)
Create a self signed certificate

### [Protect-YubiKeyOATH](Protect-YubiKeyOATH.md)
Set password

### [Register-YubikeyBIOFingerprint](Register-YubikeyBIOFingerprint.md)
Register a new fingerprint on a YubiKey Bio _or_ a YubiKey Bio Multi-Protocol Edition (MPE).

### [Remove-YubiKeyBIOFingerprint](Remove-YubiKeyBIOFingerprint.md)
Removes a selected fingerprint template from the YubiKey Bio or YubiKey Bio Multi-Protocol Edition (MPE).

### [Remove-YubikeyFIDO2Credential](Remove-YubikeyFIDO2Credential.md)
Removes a FIDO2 credential from the YubiKey.

### [Remove-YubikeyOATHAccount](Remove-YubikeyOATHAccount.md)
Removes an account from the YubiKey OATH application.

### [Remove-YubikeyOTP](Remove-YubikeyOTP.md)
Remove YubiKey OTP slot.

### [Remove-YubikeyPIVKey](Remove-YubikeyPIVKey.md)
Remove a key from a YubiKey PIV slot.

### [Rename-YubiKeyBIOFingerprint](Rename-YubiKeyBIOFingerprint.md)
Changes the template name of a registered fingerprint on the YubiKey Bio or YubiKey Bio Multi-Protocol Edition (MPE).

### [Rename-YubikeyOATHAccount](Rename-YubikeyOATHAccount.md)
Rename OATH account

### [Request-YubikeyOATHCode](Request-YubikeyOATHCode.md)
Displays TOTP / HOTP codes for YubiKey OATH credentials.

### [Request-YubikeyOTPChallange](Request-YubikeyOTPChallange.md)
Send Challaenge to YubiKey.

### [Reset-YubiKeyBioMPE](Reset-YubiKeyBioMPE.md)
Allows the user to reset the YubiKey Bio Multi-Protocol Edition (MPE) to factory settings.

### [Reset-YubiKeyFIDO2](Reset-YubiKeyFIDO2.md)
Reset a YubiKey FIDO2 device to factory settings.

### [Reset-YubikeyOATH](Reset-YubikeyOATH.md)
Reset the entire YubiKey OATH application.

### [Reset-YubikeyPIV](Reset-YubikeyPIV.md)
Resets the PIV part of your YubiKey.

### [Set-Yubikey](Set-Yubikey.md)
Allows basic YubiKey configuration.

### [Set-YubiKeyFIDO2](Set-YubiKeyFIDO2.md)
Allows settings FIDO2 options.

### [Set-YubiKeyFIDO2PIN](Set-YubiKeyFIDO2PIN.md)
Set the PIN for the FIDO2 application on the YubiKey.

### [Set-YubiKeyOATHPassword](Set-YubiKeyOATHPassword.md)
Set the password for the YubiKey OATH application.

### [Set-YubikeyOTP](Set-YubikeyOTP.md)
Configure OTP slots

### [Set-YubikeyPIV](Set-YubikeyPIV.md)
Allows the updating of PIV settings

### [Switch-YubiKeyOTP](Switch-YubiKeyOTP.md)
Switch places for the configuration of the YubiKey OTP.

### [Unblock-YubikeyPIV](Unblock-YubikeyPIV.md)
Unblock a PIN locked YubiKey PIV.

### [Unlock-Yubikey](Unlock-Yubikey.md)
Unlocks the configuration lock on the YubiKey.

### [Unprotect-YubiKeyOATH](Unprotect-YubiKeyOATH.md)
{{ Fill in the Synopsis }}

