---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Get-YubikeyFIDO2

## SYNOPSIS

Get FIDO2 information from YubiKey

## SYNTAX

### Default (Default)

```
Get-YubikeyFIDO2 [<CommonParameters>]
```

### __AllParameterSets

```
Get-YubiKeyFIDO2 [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Lists information about the FIDO2 capabilities of a YubiKey.
For instance minimum PIN length

## EXAMPLES

### Example 1

```powershell
PS C:\> Get-YubikeyFIDO2

Versions                         : {U2F_V2, FIDO_2_0, FIDO_2_1_PRE, FIDO_2_1}
Extensions                       : {credProtect, hmac-secret, largeBlobKey, credBlob…}
Aaguid                           : System.ReadOnlyMemory<Byte>[16]
Options                          : {[rk, True], [up, True], [plat, False], [alwaysUv, False]…}
MaximumMessageSize               : 1280
PinUvAuthProtocols               : {ProtocolTwo, ProtocolOne}
MaximumCredentialCountInList     : 8
MaximumCredentialIdLength        : 128
Transports                       : {nfc, usb}
Algorithms                       : {(public-key, ES256), (public-key, EdDSA), (public-key, ES384)}
MaximumSerializedLargeBlobArray  : 4096
ForcePinChange                   : False
MinimumPinLength                 : 4
FirmwareVersion                  : 329473
MaximumCredentialBlobLength      : 32
MaximumRpidsForSetMinPinLength   : 1
PreferredPlatformUvAttempts      :
UvModality                       :
Certifications                   :
RemainingDiscoverableCredentials : 100
VendorPrototypeConfigCommands    :
```

Yubikey 5.7 FIDO2 capabilities.

## PARAMETERS

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

