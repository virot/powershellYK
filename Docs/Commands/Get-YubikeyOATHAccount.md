---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Get-YubikeyOATHAccount

## SYNOPSIS

List all OATH accounts

## SYNTAX

### Default (Default)

```
Get-YubikeyOATHAccount [<CommonParameters>]
```

### __AllParameterSets

```
Get-YubiKeyOATHAccount [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

This commands list all OATH credentials registered on the Yubikey.
Both TOTP (Time-based one-time password) and HOTP (HMAC-based one-time password algorithm) will be visible.

## EXAMPLES

### Example 1

```powershell
PS C:\> Get-YubikeyOATHAccount

Type              : Totp
Algorithm         : Sha1
Issuer            : Yubico Demo
AccountName       : powershellYK
Secret            :
Digits            :
Period            : Period30
Counter           :
RequiresTouch     :
Name              : Yubico Demo:powershellYK
IsValidNameLength : True
```

Lists all TOTP Account registered on the Yubikey

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

