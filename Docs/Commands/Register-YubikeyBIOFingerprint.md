---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Register-YubikeyBIOFingerprint

## SYNOPSIS

Register a new fingerprint on a YubiKey Bio _or_ a YubiKey Bio Multi-Protocol Edition (MPE).

## SYNTAX

### Default (Default)

```
Register-YubikeyBIOFingerprint [-Name <String>] [<CommonParameters>]
```

### __AllParameterSets

```
Register-YubiKeyBIOFingerprint [-Name <string>] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Register a new fingerprint on a YubiKey Bio _or_ a YubiKey Bio Multi-Protocol Edition (MPE).

## EXAMPLES

### Example 1

```powershell
PS C:\> Register-YubikeyBIOFingerprint -Name "left index"

TemplateId                     FriendlyName
----------                     ------------
System.ReadOnlyMemory<Byte>[2] left index
```

This adds a new fingerprint to the YubiKey Bio / YubiKey Bio MPE.

## PARAMETERS

### -Name

Name of finger to register, for example: "left index" or "right index".

```yaml
Type: System.String
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

