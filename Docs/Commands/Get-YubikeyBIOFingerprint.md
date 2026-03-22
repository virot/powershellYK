---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Get-YubiKeyBIOFingerprint

## SYNOPSIS

List fingerprint templates registered on a YubiKey Bio or YubiKey Bio Multi-Protocol Edition (MPE).

## SYNTAX

### Default (Default)

```
Get-YubiKeyBIOFingerprint [<CommonParameters>]
```

### __AllParameterSets

```
Get-YubiKeyBIOFingerprint [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

List fingerprint templates registered on a YubiKey Bio or YubiKey Bio Multi-Protocol Edition (MPE).

## EXAMPLES

### Example 1

```powershell
PS C:\> Register-YubikeyBIOFingerprint -Name "left index"
Place your finger against the sensor repeatedly...
Fingerprint registered (left index).
```

Register left index finger as "left index".

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

