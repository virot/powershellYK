# powershellYK

## Feature status
| Yubikey | OpenPGP | OATH | OTP | PIV |
| --- | --- | --- | --- | --- |
| $${\color{green}100\\%}$$ | $${\color{red}0\\%}$$ | $${\color{green}80\\%}$$ | $${\color{grey}50\\%}$$ | $${\color{green}90\\%}$$  |

## Feature difference between powershellYK and Yubikey tools

### Yubikey
| Feature | powershellYK | ykman GUI | ykman CLI |
| --- | --- | --- | --- |
| Enabled / disable applications | $${\color{green}Set-Yubikey}$$ | $${\color{green}Yes}$$ | $${\color{green}yes}$$ |
| Configuration lock | $${\color{green}Lock-Yubikey}$$ $${\color{green}Unlock-Yubikey}$$ | $${\color{red}No}$$ | $${\color{green}yes}$$ |
| Configure Touch-Eject PIV | $${\color{green}Set-Yubikey}$$ $${\color{green}Unlock-Yubikey}$$ | $${\color{red}No}$$ | $${\color{green}yes}$$ |
| Configure Automatic Touch-Eject | $${\color{green}Set-Yubikey}$$ $${\color{green}Unlock-Yubikey}$$ | $${\color{red}No}$$ | $${\color{green}yes}$$ |


### FIDO2
| Feature | powershellYK | ykman CLI |
| --- | --- | --- |
| List credentials | $${\color{green}Get-YubikeyFIDO2Credentials}$$ | $${\color{green}yes}$$ |
| Remove credentials | $${\color{ews}Not implemented}$$ | $${\color{green}yes}$$ |

### OATH
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

### OTP
| Feature | powershellYK | ykman GUI | ykman CLI |
| --- | --- | --- | --- |
| Perform a challenge-response operation | $${\color{grey}Partial}$$ $${\color{green}Request-YubikeyOTPChallange}$$ | $${\color{red}No}$$ | $${\color{red}No}$$ |
| Program a challenge-response credential | $${\color{green}Set-YubikeyOTP}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ |
| Deletes the configuration stored in a slot | $${\color{green}Remove-YubikeyOTP}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Display general status of the YubiKey OTP slots | $${\color{green}Get-YubikeyOTP}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Configure a slot to be used over NDEF (NFC) | $${\color{red}Not implemented}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ |
| Update the settings for a slot | $${\color{red}Not implemented}$$ | $${\color{green}Yes}$$ | $${\color{red}No}$$ |
| Configure a static password | $${\color{green}Set-YubikeyOTP}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Swaps the two slot configurations | $${\color{green}Switch-YubikeyOTP}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Program a Yubico OTP credential | $${\color{lightgrrey}Set-YubikeyOTP}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |

### PIV
| Feature | powershellYK | ykman GUI | ykman CLI | yubico-piv-tool |
| --- | --- | --- | --- | --- |
| Generate CSR with Attestation | $${\color{green}dasdas}$$| $${\color{red}No}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ |
| Generate Attestation cert | $${\color{green}Yes}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Sign certificate request | $${\color{green}Build\-YubikeyPIVSignedCertificate}$$ | $${\color{red}No}$$ | $${\color{red}No}$$ | $${\color{red}No}$$ |
| Delete keys in slot | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |

### just in powershellYK

Validate yubikey attestion certificates

Validate yubikey certificate request with builtin attestion

Calculate altSecurityIdentities for AD and authoriziedkeys for SSH.

