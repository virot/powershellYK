---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Get-YubikeyOATHCredential

## SYNOPSIS
List all OATH credentials

## SYNTAX

```
Get-YubikeyOATHCredential [<CommonParameters>]
```

## DESCRIPTION
This commands list all OATH credentials registered on the Yubikey.
Both TOTP (Time-based one-time password) and HOTP (HMAC-based one-time password algorithm) will be visible.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-YubikeyOATHCredential

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

Lists all TOTP credentials registered on the Yubikey

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
