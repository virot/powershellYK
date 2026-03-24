---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Reset-YubiKeyFIDO2

## SYNOPSIS

Reset a YubiKey FIDO2 device to factory settings.

## SYNTAX

### Default (Default)

```
Reset-YubiKeyFIDO2 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### __AllParameterSets

```
Reset-YubiKeyFIDO2 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Resets the YubiKey FIDO2 applet to factory settings. This will remove all stored credentials and reset the applet to factory settings.
This REQUIRES the YubiKey to be (re)inserted at a maximum `5` seconds _before_ running the command.

## EXAMPLES

### Example 1

```powershell
PS C:\> Reset-YubikeyFIDO2
```

Resets the YubiKey FIDO2 applet to factory settings.

## PARAMETERS

### -Confirm

Prompts you for confirmation before running the cmdlet.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases:
- cf
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

### -WhatIf

Runs the command in a mode that only reports what would happen without performing the actions.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases:
- wi
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

