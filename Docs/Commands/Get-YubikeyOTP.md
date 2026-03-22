---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Get-YubikeyOTP

## SYNOPSIS

YubiKey OTP Information

## SYNTAX

### Default (Default)

```
Get-YubikeyOTP [<CommonParameters>]
```

### __AllParameterSets

```
Get-YubiKeyOTP [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Command to retrive information about the YubiKey OTP configuration. As the Yubikey OTP does not allow information about what the slots are confgured to contain, this is not listed.

## EXAMPLES

### Example 1

```powershell
PS C:\> Get-YubikeyOTP

Slot       Configured RequiresTouch
----       ---------- -------------
ShortPress       True          True
LongPress        True          True
```

This command will display the current configuration of the Yubikey OTP.

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

