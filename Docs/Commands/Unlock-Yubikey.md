---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Unlock-Yubikey

## SYNOPSIS

Unlocks the configuration lock on the YubiKey.

## SYNTAX

### Default (Default)

```
Unlock-Yubikey -LockCode <Byte[]> [<CommonParameters>]
```

### __AllParameterSets

```
Unlock-YubiKey -LockCode <byte[]> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Allow the Yubikey to be configured once more.

## EXAMPLES

### Example 1

```powershell
PS C:\> $Lockcode = [byte[]](1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)
PS C:\> Unlock-Yubikey -LockCode $Lockcode
WARNING: Please remove and reinsert Yubikey
```

Removes the configuration lock on the Yubikey

## PARAMETERS

### -LockCode

Lock Code for YubiKey

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

