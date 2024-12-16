# powershellYK

## Feature status
| Yubikey | OpenPGP | OATH | OTP | PIV |
| --- | --- | --- | --- | --- |
| $${\color{green}100\\%}$$ | $${\color{red}0\\%}$$ | $${\color{green}80\\%}$$ | $${\color{grey}50\\%}$$ | $${\color{green}90\\%}$$  |

## Feature difference between powershellYK and Yubikey tools

### Yubikey
| Feature | powershellYK | ykman GUI | ykman CLI | Yubico Authenticator |
| --- | --- | --- | --- |--- |
| Enabled / disable applications | $${\color{green}Set-Yubikey}$$ | $${\color{green}Yes}$$ | $${\color{green}yes}$$ |$${\color{green}yes}$$ |
| Configuration lock | $${\color{green}Lock-Yubikey}$$ $${\color{green}Unlock-Yubikey}$$ | $${\color{red}No}$$ | $${\color{green}yes}$$ | $${\color{red}no}$$ |
| Configure Touch-Eject PIV | $${\color{green}Set-Yubikey}$$ $${\color{green}Unlock-Yubikey}$$ | $${\color{red}No}$$ | $${\color{green}yes}$$ | $${\color{red}no}$$ |
| Configure Automatic Touch-Eject | $${\color{green}Set-Yubikey}$$ $${\color{green}Unlock-Yubikey}$$ | $${\color{red}No}$$ | $${\color{green}yes}$$ | $${\color{red}no}$$ |
| Restrict NFC | $${\color{green}yes}$$ | $${\color{red}No}$$ | $${\color{green}yes}$$ | $${\color{red}no}$$ |

### FIDO (U2F & FIDO2)
| Feature | powershellYK | ykman GUI | ykman CLI | Yubico Authenticator |
| --- | --- | --- |--- |--- |
| List passkey credentials | $${\color{green}Get-YubikeyFIDO2Credentials}$$ | $${\color{red}no}$$ | $${\color{green}yes}$$ | $${\color{green}yes}$$ |
| Remove passkey credentials | $${\color{ews}Not implemented}$$ | $${\color{red}no}$$ | $${\color{green}yes}$$ | $${\color{green}yes}$$ |
| Force PIN change | $${\color{green}yes}$$ | $${\color{red}no}$$ | $${\color{green}yes}$$ | $${\color{green}yes}$$ |

### OATH (TOTP & HOTP)
| Feature | powershellYK | ykman GUI | ykman CLI | Yubico Authenticator |
| --- | --- | --- | --- | --- |
| Basic info | $${\color{green}Get-YubikeyOATH}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Set password | $${\color{green}Protect-YubikeyOATH}$$ $${\color{green}Unprotect-YubikeyOATH}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{grey}N/A}$$ |
| Remember / forget password on computer | $${\color{red}Not implemented}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{grey}N/A}$$ |
| List accounts | $${\color{green}Get-YubikeyOATHAccount}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Generate codes | $${\color{green}Request-YubikeyOATHCode}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Rename accounts | $${\color{green}Rename-YubikeyOATHAccount}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Remove accounts | $${\color{green}Rename-YubikeyOATHAccount}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Reset application | $${\color{green}Reset-YubikeyOATH}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |

### OTP (YubiOTP, Challenge-Response) & Static Password
| Feature | powershellYK | ykman GUI | ykman CLI | Yubico Authenticator |
| --- | --- | --- | --- | --- |
| Perform a challenge-response operation | $${\color{grey}Partial}$$ $${\color{green}Request-YubikeyOTPChallange}$$ | $${\color{red}No}$$ | $${\color{red}No}$$ | $${\color{red}No}$$ |
| Program a challenge-response credential | $${\color{green}Set-YubikeyOTP}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Deletes the configuration stored in a slot | $${\color{green}Remove-YubikeyOTP}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Display general status of the YubiKey OTP slots | $${\color{green}Get-YubikeyOTP}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Configure a slot to be used over NDEF (NFC) | $${\color{red}Not implemented}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{red}No}$$ |
| Update the settings for a slot | $${\color{red}Not implemented}$$ | $${\color{green}Yes}$$ | $${\color{red}No}$$ | $${\color{red}No}$$ |
| Configure a static password | $${\color{green}Set-YubikeyOTP}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Swaps the two slot configurations | $${\color{green}Switch-YubikeyOTP}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Program a YubiOTP credential | $${\color{lightgrrey}Set-YubikeyOTP}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |

### PIV
| Feature | powershellYK | ykman GUI | ykman CLI | Yubico Authenticator | yubico-piv-tool |
| --- | --- | --- | --- | --- | --- |
| Generate CSR with Attestation | $${\color{green}New-YubikeyPIVCSR}$$| $${\color{red}No}$$ | $${\color{red}No}$$ | $${\color{red}No}$$ |$${\color{green}Yes}$$ |
| Generate Attestation cert | $${\color{green}Assert-YubikeyPIV}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{red}No}$$ |$${\color{green}Yes}$$ |
| Sign certificate request | $${\color{green}Build\-YubikeyPIVSignedCertificate}$$ | $${\color{red}No}$$ | $${\color{red}No}$$ | $${\color{red}No}$$ |$${\color{red}No}$$ |
| Delete keys in slot | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |$${\color{green}Yes}$$ |

### Only in powershellYK

- Validate YubiKey attestion certificates.
- Validate YubiKey certificate signing requests (CSRs) with _built-in_ attestion.
- Calculate the `altSecurityIdentities` attribute for use with SSH and AD(DS).

