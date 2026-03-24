---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Rename-YubiKeyBIOFingerprint

## SYNOPSIS

Changes the template name of a registered fingerprint on the YubiKey Bio or YubiKey Bio Multi-Protocol Edition (MPE).

## SYNTAX

### Rename using Name

```
Rename-YubiKeyBIOFingerprint -Name <String> -NewName <String> [<CommonParameters>]
```

### Rename using ID

```
Rename-YubiKeyBIOFingerprint -ID <String> -NewName <String> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

You can update the friendly name of a fingerprint on the yubikey Bio.

## EXAMPLES

### Example 1

```powershell
PS C:\> Get-YubikeyBIOFingerprint

ID   Name
--   ----
23FC left index

PS C:\> Rename-YubikeyBIOFingerprint -ID 23FC -NewName "left index finger"
Fingerprint renamed (left index finger).
```

Changes the friendly name of the fingerprint with name "left index" to "left index finger".

### Example 2

```powershell
PS C:\> Get-YubikeyBIOFingerprint

ID   Name
--   ----
23FC left index

PS C:\> Rename-YubikeyBIOFingerprint -Name "left" -NewName "thumb"
Fingerprint renamed (thumb).
```

Changes the friendly name of the fingerprint with name "left index to "thumb".

## PARAMETERS

### -ID

ID of fingerprint to rename

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Rename using ID
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Name

Friendly name of fingerprint to rename

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Rename using Name
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -NewName

New friendly name

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Rename using ID
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: Rename using Name
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

