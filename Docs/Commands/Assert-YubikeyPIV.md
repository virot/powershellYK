---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Assert-YubiKeyPIV

## SYNOPSIS

Create attestation certificate

## SYNTAX

### ExportToFile

```
Assert-YubiKeyPIV -Slot <PIVSlot> -OutFile <FileInfo> [<CommonParameters>]
```

### DisplayOnScreen

```
Assert-YubiKeyPIV -Slot <PIVSlot> [-PEMEncoded] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Create and export attestation certificate for a slot

## EXAMPLES

### Example 1

```powershell
PS C:\> Assert-YubikeyPIV -Slot 0x9a -OutFile attestation.cer
```

Creates and exports the attestation certificate for slot 0x9a

## PARAMETERS

### -OutFile

Location of the attestation certificate

```yaml
Type: System.IO.FileInfo
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ExportToFile
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PEMEncoded

Encode output as PEM

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: DisplayOnScreen
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Slot

Yubikey PIV Slot

```yaml
Type: powershellYK.PIV.PIVSlot
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ExportToFile
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: DisplayOnScreen
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

