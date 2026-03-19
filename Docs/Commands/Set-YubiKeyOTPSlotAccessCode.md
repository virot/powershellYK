---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Set-YubiKeyOTPSlotAccessCode

## SYNOPSIS

Sets, changes or removes the OTP slot access code for a YubiKey.
he access code protects OTP slot configurations from unauthorized modifications.

## SYNTAX

### SetNewAccessCode

```
Set-YubiKeyOTPSlotAccessCode -Slot <Slot> [-AccessCode <String>] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### ChangeAccessCode

```
Set-YubiKeyOTPSlotAccessCode -Slot <Slot> -AccessCode <String> -CurrentAccessCode <String> [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

### RemoveAccessCode

```
Set-YubiKeyOTPSlotAccessCode -Slot <Slot> -CurrentAccessCode <String> [-RemoveAccessCode] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Sets, changes or removes the OTP slot access code for a YubiKey.
The access code protects OTP slot configurations from unauthorized modifications.
Access codes are 6 bytes in length, provided as 12-character hex strings.

## EXAMPLES

### Example 1

```powershell
PS C:\> Set-YubiKeySlotAccessCode -Slot LongPress -AccessCode "010203040506"
```

Set a new access code for a slot (when no access code exists)

### Example 2

```powershell
PS C:\> Set-YubiKeyOTPSlotAccessCode -Slot ShortPress -CurrentAccessCode "010203040506" -AccessCode "060504030201"
```

Change an existing slot access code

### Example 3

```powershell
PS C:\> Set-YubiKeyOTPSlotAccessCode -Slot LongPress -CurrentAccessCode "010203040506" -RemoveAccessCode
```

Remove slot access code protection (set to all zeros)

## PARAMETERS

### -AccessCode

New access code (12-character hex string)

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: SetNewAccessCode
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ChangeAccessCode
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

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
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangeAccessCode
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: RemoveAccessCode
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RemoveAccessCode

Remove access code protection

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: RemoveAccessCode
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

Yubikey OTP Slot

```yaml
Type: Yubico.YubiKey.Otp.Slot
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
AcceptedValues:
- None
- ShortPress
- LongPress
HelpMessage: ''
```

### -WhatIf

Shows what would happen if the cmdlet runs.
The cmdlet is not run.
Runs the command in a mode that only reports what would happen without performing the actions.
Runs the command in a mode that only reports what would happen without performing the actions.
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

