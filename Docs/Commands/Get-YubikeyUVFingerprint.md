---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Get-YubikeyUVFingerprint

## SYNOPSIS
List fingerprint templates registered on a YubiKey Bio or YubiKey Bio Multi-Protocol Edition (MPE).

## SYNTAX

```
Get-YubikeyUVFingerprint [<CommonParameters>]
```

## DESCRIPTION
List fingerprint templates registered on a YubiKey Bio or YubiKey Bio Multi-Protocol Edition (MPE).

## EXAMPLES

### Example 1
```powershell
PS C:\> Register-YubikeyUVFingerprint -Name "left index"
Place your finger against the sensor repeatedly...
Fingerprint registered (left index).
```
Register left index finger as "left index".

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
