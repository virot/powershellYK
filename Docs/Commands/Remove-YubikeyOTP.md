---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Remove-YubikeyOTP

## SYNOPSIS

Remove YubiKey OTP slot.

## SYNTAX

### Default (Default)

```
Remove-YubikeyOTP -Slot <Slot> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Remove

```
Remove-YubiKeyOTP -Slot <Slot> [-CurrentAccessCode <string>] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Remove the OTP configuration from a slot on the Yubikey

## EXAMPLES

### Example 1

```powershell
PS C:\> Remove-YubikeyOTP -Slot 1
```

Removes the OTP configuration from slot 1 (Short press)

### Example 2

```powershell
PS C:\> $Slot = [Yubico.YubiKey.Otp.Slot]::ShortPress
PS C:\> Remove-YubikeyOTP -Slot $Slot
```

Removes the OTP configuration from slot 1 (Short press)

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

### -CurrentAccessCode

Current access code (12-character hex string)

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Remove
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

YubiOTP Slot

```yaml
Type: Yubico.YubiKey.Otp.Slot
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Remove
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- None
- ShortPress
- LongPress
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

