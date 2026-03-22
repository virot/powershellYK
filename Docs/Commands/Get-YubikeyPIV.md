---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Get-YubikeyPIV

## SYNOPSIS

Gets information about the PIV module and specific slots.

## SYNTAX

### Default (Default)

```
Get-YubikeyPIV [-Slot <PIVSlot>] [<CommonParameters>]
```

### __AllParameterSets

```
Get-YubiKeyPIV [-Slot <PIVSlot>] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Gets information from both the yubikey and specific slots.

## EXAMPLES

### Example 1

```powershell
PS C:\> Get-YubikeyPIV

PinRetriesLeft       : 8
PinRetries           : 8
PukRetriesLeft       : 4
PukRetries           : 4
CHUID                : 31-A5-EC-44-7F-1F-7E-C5-EA-4C-A8-3C-BB-28-F9-01
SlotsWithPrivateKeys : {154, 158}
```

Displays the number of retires left and total for PIN and PUK code. Aswell as Slots with private keys and the CHUID.

### Example 2

```powershell
PS C:\> Get-YubikeyPIV -Slot 0x9e

Slot        : 0x9E
KeyStatus   : Generated
Algorithm   : EccP384
PinPolicy   : Never
TouchPolicy : Never
Subjectname : CN=powershellYK
Issuer      : CN=powershellYK
NotBefore   : 2024-06-12 21:36:26
NotAfter    : 2034-06-12 21:36:26
```

Displays information about the PIV slot and any contained certificate

## PARAMETERS

### -Slot

Retrive a info from specific slot

```yaml
Type: System.Nullable`1[powershellYK.PIV.PIVSlot]
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

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

