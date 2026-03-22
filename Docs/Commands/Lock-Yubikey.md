---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Lock-Yubikey

## SYNOPSIS

Lock the YubiKey configuration

## SYNTAX

### Default (Default)

```
Lock-Yubikey -LockCode <Byte[]> [<CommonParameters>]
```

### __AllParameterSets

```
Lock-YubiKey -LockCode <byte[]> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Locks the YubiKey behind a 16 byte lock code. This lock code is required to unlock the Yubikey configuration.

## EXAMPLES

### Example 1

```powershell
PS C:\> $Lockcode = [byte[]](1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)
PS C:\> Lock-Yubikey -LockCode $Lockcode
WARNING: Please remove and reinsert Yubikey
```

Locks the configuration so it requires a hex code of 0102030405060708090A0B0C0D0E0F10 to unlock.

## PARAMETERS

### -LockCode

LockCode for Yubikey

```yaml
Type: System.Byte[]
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: true
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

