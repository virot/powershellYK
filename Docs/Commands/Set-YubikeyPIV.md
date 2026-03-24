---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Set-YubikeyPIV

## SYNOPSIS

Allows the updating of PIV settings

## SYNTAX

### ChangeRetries

```
Set-YubiKeyPIV -PinRetries <byte> -PukRetries <byte> [-KeepPukUnlocked] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### ChangePIN

```
Set-YubiKeyPIV -PIN <securestring> -NewPIN <securestring> [-ChangePIN] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### UnblockPIN

```
Set-YubiKeyPIV -NewPIN <securestring> -PUK <securestring> [-UnblockPIN] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### ChangePUK

```
Set-YubiKeyPIV -PUK <securestring> -NewPUK <securestring> [-ChangePUK] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### ChangeManagement

```
Set-YubiKeyPIV -ManagementKey <psobject> -NewManagementKey <psobject> -Algorithm <PivAlgorithm>
 -TouchPolicy <PivTouchPolicy> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### newCHUID

```
Set-YubiKeyPIV -newCHUID [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Set Managementkey to PIN protected

```
Set-YubiKeyPIV -PINProtectedManagementkey [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Allows the modification of PIV settings like: PIN, PUK, ManagementKey and CHUID.

## EXAMPLES

### Example 1

```powershell
PS C:\> Set-YubikeyPIV -PinRetries 8 -PukRetries 4
WARNING: PIN and PUK codes reset to default, remember to change.
```

Updates the PIV to 8 PIN retries and 4 PUK retries.

### Example 3

```powershell
PS C:\GIT-VS\Yubikey_Powershell> Set-YubikeyPIV -ChangePIN

cmdlet Set-YubikeyPIV at command pipeline position 1
Supply values for the following parameters:
(Type !? for Help.)
PIN: ******
NewPIN: ******
```

Change PIN with a easy way of requesting the new codes.

## PARAMETERS

### -Algorithm

Algoritm

```yaml
Type: Yubico.YubiKey.Piv.PivAlgorithm
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangeManagement
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- TripleDES
- AES128
- AES192
- AES256
HelpMessage: ''
```

### -ChangePIN

Change the PIN

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangePIN
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ChangePUK

Change the PUK (PIN Unblocking Key)

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangePUK
  Position: Named
  IsRequired: false
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

### -KeepPukUnlocked

Keep PUK unlocked

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangeRetries
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ManagementKey

Current Management key

```yaml
Type: System.Management.Automation.PSObject
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangeManagement
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -newCHUID

Generate new CHUID

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: newCHUID
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -NewManagementKey

New Management key

```yaml
Type: System.Management.Automation.PSObject
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangeManagement
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -NewPIN

New PIN

```yaml
Type: System.Security.SecureString
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangePIN
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: UnblockPIN
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -NewPUK

New PUK

```yaml
Type: System.Security.SecureString
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangePUK
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PIN

Current PIN

```yaml
Type: System.Security.SecureString
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangePIN
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PINProtectedManagementkey

PIN protect the Management key

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Set Managementkey to PIN protected
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PinRetries

Change the number of PIN retries

```yaml
Type: System.Nullable`1[System.Byte]
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangeRetries
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PUK

Current PUK

```yaml
Type: System.Security.SecureString
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: UnblockPIN
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ChangePUK
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PukRetries

Change the number of PUK retries

```yaml
Type: System.Nullable`1[System.Byte]
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangeRetries
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -TouchPolicy

Touch policy

```yaml
Type: Yubico.YubiKey.Piv.PivTouchPolicy
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChangeManagement
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- Default
- Never
- Always
- Cached
HelpMessage: ''
```

### -UnblockPIN

Unblock the PIN

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: UnblockPIN
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

