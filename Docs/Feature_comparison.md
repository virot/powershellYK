# powershellYK

## Feature difference between powershellYK and Yubikey tools

### Yubikey configuration
| Feature | powershellYK | YubiKey Manager (GUI) | YubiKey Manager (CLI) | Yubico Authenticator |
| :--- | --- | --- | --- | --- |
| **Toggle applications** | 🟢 | 🟢 | 🟢 |🟢 |
| **Toggle interfaces** | 🟢 | 🟢 | 🟢 |🟢 |
| **Set Configuration lock** | 🟢 | 🔴 | 🟢 | 🔴 |
| **Configure Touch-Eject PIV** | 🟢 | 🔴 | 🟢 | 🔴 |
| **Configure Auto Touch-Eject** | 🟢 | 🔴 | 🟢 | 🔴 |
| **Restrict NFC** | 🟢 | 🔴 | 🟢 | 🔴 |

### FIDO (U2F & FIDO2)
| Feature | powershellYK | YubiKey Manager (GUI) | YubiKey Manager (CLI) | Yubico Authenticator |
| :--- | --- | --- | --- | --- |
| **Set PIN** | 🟢 | 🟢 | 🟢 | 🟢 |
| **Set minimum PIN length** | 🟢 | 🟢 | 🟢 | 🔴 |
| **Force PIN change** | 🟢 | 🔴 | 🟢 | 🔴 |
| **Create passkey credentials** | 🟢 | 🔴 | 🔴 | 🔴 |
| **List passkey credentials** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Remove passkey credentials** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Reset applet** | 🟢 | 🟢 | 🟢 | 🟢 |


### OATH (TOTP)
| Feature | powershellYK | YubiKey Manager (GUI) | YubiKey Manager (CLI) | Yubico Authenticator |
| :--- | --- | --- | --- | --- |
| **Basic info** | 🟢 | 🔴 | 🟢 |🟢 |
| **Set password** | 🟢 | 🔴 | 🟢 | 🟢 |
| **List accounts** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Generate One Time Password** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Rename accounts** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Remove accounts** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Reset applet** | 🟢 | 🔴 | 🟢 | 🟢 |

### YubiOTP, HOTP, Challenge-Response & Static Password
| Feature | powershellYK | YubiKey Manager (GUI) | YubiKey Manager (CLI) | Yubico Authenticator |
| :--- | --- | --- | --- | --- |
| **Program challenge-response** | 🟢 | 🔴 | 🟢 | 🟢 |
| **Perform challenge-response** | 🔴 | 🔴 | 🔴 | 🔴 |
| **Delete slot configuration** | 🟢 | 🟢 | 🟢 | 🟢 |
| **Protect slot configuration** | 🟢 | 🔴 | 🟢 | 🔴 |
| **Display slot status** | 🟢 | 🟢 | 🟢 | 🟢 |
| **Configure static password** | 🟢 | 🟢 | 🟢 | 🟢 |
| **Configure NDEF (NFC) slot** | 🔴| 🔴 | 🟢 | 🔴 |
| **Update slot settings** | 🔴 | 🟢 | 🔴 | 🔴 |
| **Swap slot configurations** | 🟢 | 🟢 | 🟢 | 🟢 |
| **Program YubiOTP** | 🔴 | 🟢 | 🟢 | 🟢 |

### PIV
| Feature | powershellYK | YubiKey Manager (GUI) | YubiKey Manager (CLI) | Yubico Authenticator | yubico-piv-tool |
| :--- | --- | --- | --- | --- | --- |
| **Set PIN** | 🟢 | 🟢 | 🟢 |🟢 | 🟢 |
| **Set PUK** | 🟢 | 🟢 | 🟢 |🟢 | 🟢 |
| **Set Management Key** | 🟢 | 🟢 | 🟢 |🟢 | 🟢 |
| **Generate keys in slot** | 🟢 | 🟢 | 🟢 |🟢 | 🟢 |
| **Move keys between slot** | 🟢 | 🔴 | 🟢 |🔴 | 🟢 |
| **Delete keys in slot** | 🟢 | 🟢 | 🟢 | 🟢 |🟢 |
| **Generate CSR with Attestation** | 🟢 | 🔴 | 🔴 |🔴 | 🟢 |
| **Generate CSR with Attestation** | 🟢 | 🔴 | 🔴 |🔴 | 🟢 |
| **Generate Attestation certificate** | 🟢 | 🔴 | 🟢 | 🔴 |🟢 |
| **Sign certificate request** | 🟢 | 🔴 | 🔴 | 🔴 |🔴 |
| **Reset applet** | 🟢 | 🟢 | 🟢 |🟢 | 🟢 |


## Only in powershellYK
_The following are custom features available only in powershellYK:_
- Validate YubiKey attestion certificates.
- Validate YubiKey certificate signing requests (CSRs) with _built-in_ attestion.
- Calculate the `altSecurityIdentities` attribute for use with SSH and AD(DS).
