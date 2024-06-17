# powershellYK

PowershellYK is thirdparty powershell module using the official Yubikey .NET SDK to allow management of Yubikeys from Powershell. This module does not depend on any installation of other Yubico tools.


## 💾 Installation
The powershellYK module is available from the (PowershellGallery](https://www.powershellgallery.com/packages/powershellYK).
The easiest way to install the module is:

1. Open PowerShell
2. Execute the following command: ```Install-Module powershellYK```
3. Press ```Y``` when prompted to proceed with installation.

## Feature difference between powershellYK and Yubikey tools

### FIDO2
| Feature | powershellYK | ykman GUI | ykman CLI |
| --- | --- | --- | --- |
| List credentials | $${\color{green}Yes}$$ | $${\color{red}No}$$ | $${\color{green}yes}$$ |
| Remove credentials | $${\color{green}Yes}$$ | $${\color{red}No}$$ | $${\color{green}yes}$$ |

### OATH
| Feature | powershellYK | ykman GUI | ykman CLI | Yubico Authenticator |
| --- | --- | --- | --- | --- |
| List accounts | $${\color{green}Yes}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Generate codes | $${\color{green}Yes}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Rename accounts | $${\color{green}Yes}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |

### PIV
| Feature | powershellYK | ykman GUI | ykman CLI | yubico-piv-tool |
| --- | --- | --- | --- | --- |
| Generate CSR with Attestation | $${\color{green}Yes}$$ | $${\color{red}No}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ |
| Generate Attestation cert | $${\color{green}Yes}$$ | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |
| Delete keys in slot | $${\color{red}No}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ | $${\color{green}Yes}$$ |

## 📖 Usage
For usage instructions, please refer to online Cmdlets documentation [here](https://github.com/virot/powershellYK/blob/master/Documentation/Commands/powershellYK.md)




