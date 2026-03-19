---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# New-YubiKeyPIVKey

## SYNOPSIS

Create a new private key

## SYNTAX

### Default (Default)

```
New-YubiKeyPIVKey [-Slot] <PIVSlot> -Algorithm <PivAlgorithm> [-PinPolicy <PivPinPolicy>]
 [-TouchPolicy <PivTouchPolicy>] [-PassThru] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### __AllParameterSets

```
New-YubiKeyPIVKey [-Slot] <PIVSlot> -Algorithm <KeyType> [-PinPolicy <PivPinPolicy>]
 [-TouchPolicy <PivTouchPolicy>] [-PassThru] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

This cmdlet will create a new key, this can be done with either RSA or ECC keys.

## EXAMPLES

### Example 1

```powershell
PS C:\> New-YubikeyPIVKey -Slot 0x9a -Algorithm EccP384
```

Creates a new Elliptic curve P-384 key in slot 0x9a.

### Example 2

```powershell
PS C:\> New-YubikeyPIVKey -Slot 0x9a -Algorithm RSA2048 -PinPolicy Never
```

Create a RSA2048 in slot 0x9a with a PIN policy of never.

### Example 3

```powershell
PS C:\> New-YubikeyPIVKey -Slot 0x9a -Algorithm EccP384 -TouchPolicy Cached
```

Create a RSA2048 in slot 0x9a with a touch policy of cached

## PARAMETERS

### -Algorithm

What algorithm to use, dependent on YubiKey firmware.

```yaml
Type: Yubico.YubiKey.Cryptography.KeyType
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
- Rsa1024
- Rsa2048
- Rsa3072
- Rsa4096
- EccP256
- EccP384
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

### -PassThru

Returns an object that represents the item with which you're working. By default, this cmdlet doesn't generate any output.

```yaml
Type: System.Management.Automation.SwitchParameter
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

### -PinPolicy


Pin policy

```yaml
Type: Yubico.YubiKey.Piv.PivPinPolicy
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
AcceptedValues:
- None
- Never
- Once
- Always
- MatchOnce
- MatchAlways
- Default
HelpMessage: ''
```

### -Slot
What slot to create a new key in

```yaml
Type: powershellYK.PIV.PIVSlot
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: 0
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
- Name: (All)
  Position: Named
  IsRequired: false
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

