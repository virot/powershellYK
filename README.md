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
| --- | --- | --- | --- | --- |
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

### Online documentation of Cmdlets
[powershellYK Cmdlet documentation](https://github.com/virot/powershellYK/blob/master/Documentation/Commands/powershellYK.md)

### Output AltSecurityIdentities attribute(s)
To output applicable AltSecurityIdentities attributes for a given certificate: ```convertto-AltSecurity -Certificate <path>\<filename>```.

💡To instead write attributes to file, use: ```convertto-AltSecurity -Certificate <path>\<filename> > <path>\<output_filename>```.


#### Example command output:
Here is an example command execution with its corresponding result:

```
PS C:\> convertto-AltSecurity -Certificate C:\alice.crt

sshAuthorizedkey       : ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQC+x/1FgHAbp1UF25emOJ5CNZ7fSYnKz6UtOT5KkndBnZmxMW5XydXyz3gloY24RQIRvzmZfe0u0Wv
                         tjZcUqQDFXsQUnPjwi5h2VZ8vFQd+ZcQk5sDkRdp3qSsdXA/rFvg/G1CQrBEXGbJ5Qvy0U0OFdbVDP5ucRj5bk35flETxDeuZnzEdJZSHCDJ/3UgknL
                         XPOMoVkL6MLbka7/t/tFeWCSCVvuQieWArK/5AvnIv3rU8mWLU8eA8F24ZfJOhwH3oiW6+DJwN1yjL3brjei9t6EBF6mqBOsBK2hzNtNfZtrAPwiZx2
                         GtCAE9CPjndp/elA8na6mwBPHBzinqgyTBr CN=Alice Smith
X509IssuerSubject      : X509:<I>CN=Alice Smith<S>CN=Alice Smith
X509SubjectOnly        : X509:<S>CN=Alice Smith
X509RFC822             : 
X509IssuerSerialNumber : X509:<I>CN=Alice Smith<SR>1D78F490BEEA12BFD3D6FAB6A217938E5AFC8B54
X509SKI                : X509:<SKI>
X509SHA1PublicKey      : X509:<SHA1-PUKEY>D82C910ABBA48A03600DB0DC5ABF2C15B24C215D
```

**Note**: Only ```X509IssuerSerialNumber```, ```X509SKI``` and ```X509SHA1PublicKey``` are considered _strong_ bindings (by [Microsoft](https://support.microsoft.com/en-us/topic/kb5014754-certificate-based-authentication-changes-on-windows-domain-controllers-ad2c23b0-15d8-4340-a468-4d4f3b188f16)).




