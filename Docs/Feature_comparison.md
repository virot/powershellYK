# powershellYK

## Feature difference between powershellYK and Yubikey tools

### Yubikey configuration
| Feature | powershellYK | ykman GUI | ykman CLI | Yubico Authenticator |
| :--- | --- | --- | --- | --- |
| **Toggle applications** | 🟢 | 🟢 | 🟢 |🟢 |
| **Configuration lock** | 🟢 | 🔴 | 🟢 | 🔴 |
| **Configure Touch-Eject PIV** | 🟢 | 🔴 | 🟢 | 🔴 |
| **Configure Automatic Touch-Eject** | 🟢 | 🔴 | 🟢 | 🔴 |
| **Restrict NFC** | 🟢 | 🔴 | 🟢 | 🔴 |

### FIDO (U2F & FIDO2)
| Feature | powershellYK | ykman GUI | ykman CLI | Yubico Authenticator |
| :--- | --- | --- | --- | --- |
| **Set PIN** | 🟢 | 🟢 | 🟢 | 🟢 |
| **Set minimum PIN length** | 🟢 | 🟢 | 🟢 | 🔴 |
| **Force PIN change** | 🟢 | 🔴 | 🟢 | 🔴 |
| **List passkey credentials** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Remove passkey credentials** | 🔴 | 🔴 | 🟢 | 🟢 |
| **Reset applet** | 🟢 | 🟢 | 🟢 | 🟢 |


### OATH (TOTP & HOTP)
| Feature | powershellYK | ykman GUI | ykman CLI | Yubico Authenticator |
| :--- | --- | --- | --- | --- |
| **Basic info** | 🟢 | 🔴 | 🟢 |🟢 |
| **Set password** | 🟢 | 🔴 | 🟢 | 🟢 |
| **List accounts** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Generate OTP** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Rename accounts** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Remove accounts** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Reset application** | 🟢 | 🔴 | 🟢 | 🟢 |

### YubiOTP, Challenge-Response & Static Password
| Feature | powershellYK | ykman GUI | ykman CLI | Yubico Authenticator |
| :--- | --- | --- | --- | --- |
| **Perform a challenge-response operation** | 🔴 | 🔴 | 🔴 | 🔴 |
| **Program a challenge-response credential** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Delete configuration stored in a slot** | 🟢 | 🟢 | 🟢 | 🟢 |
| **Display general status of the YubiKey OTP slots** | 🟢 | 🟢 | 🟢 | 🟢 |
| **Configure a slot to be used over NDEF (NFC)** | 🔴| 🔴 | 🟢 | 🔴 |
| **Update the settings for a slot** | 🔴 | 🟢 | 🔴 | 🔴 |
| **Configure a static password** | 🟢 | 🟢 | 🟢 | 🟢 |
| **Swap slot configurations** | 🟢 | 🟢 | 🟢 | 🟢 |
| **Program a YubiOTP credential** | 🔴 | 🟢 | 🟢 | 🟢 |

### PIV
| Feature | powershellYK | ykman GUI | ykman CLI | Yubico Authenticator | yubico-piv-tool |
| :--- | --- | --- | --- | --- | --- |
| **Generate CSR with Attestation** | 🟢 | 🔴 | 🔴 |🔴 | 🟢 |
| **Generate Attestation certificate** | 🟢 | 🔴 | 🟢 | 🔴 |🟢 |
| **Sign certificate request** | 🟢 | 🔴 | 🔴 | 🔴 |🔴 |
| **Delete keys in slot** | 🟢 | 🟢 | 🟢 | 🟢 |🟢 |

### Only in powershellYK
_The following are custom features available only in powershellYK:_
- Validate YubiKey attestion certificates.
- Validate YubiKey certificate signing requests (CSRs) with _built-in_ attestion.
- Calculate the `altSecurityIdentities` attribute for use with SSH and AD(DS).
