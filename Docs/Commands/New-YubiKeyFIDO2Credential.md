---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# New-YubiKeyFIDO2Credential

## SYNOPSIS

Creates a new FIDO2 credential on the connected YubiKey.
For more complete examples see: https://github.com/virot/powershellYK/tree/master/Docs/Examples

## SYNTAX

### UserData-HostData

```
New-YubiKeyFIDO2Credential -RelyingPartyID <string> -Username <string> -UserID <byte[]>
 -Challenge <Challenge> [-RelyingPartyName <string>] [-UserDisplayName <string>]
 [-Discoverable <bool>] [-RequestedAlgorithms <List`1[CoseAlgorithmIdentifier]>] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

### UserEntity-HostData

```
New-YubiKeyFIDO2Credential -RelyingPartyID <string> -Challenge <Challenge> -UserEntity <UserEntity>
 [-RelyingPartyName <string>] [-Discoverable <bool>]
 [-RequestedAlgorithms <List`1[CoseAlgorithmIdentifier]>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### UserData-RelyingParty

```
New-YubiKeyFIDO2Credential -RelyingParty <RelyingParty> -Username <string> -UserID <byte[]>
 -Challenge <Challenge> [-UserDisplayName <string>] [-Discoverable <bool>]
 [-RequestedAlgorithms <List`1[CoseAlgorithmIdentifier]>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### UserEntity-RelyingParty

```
New-YubiKeyFIDO2Credential -RelyingParty <RelyingParty> -Challenge <Challenge>
 -UserEntity <UserEntity> [-Discoverable <bool>]
 [-RequestedAlgorithms <List`1[CoseAlgorithmIdentifier]>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

{{ Fill in the Description }}

## EXAMPLES

### Example 1

```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -Challenge

Challange.

```yaml
Type: powershellYK.FIDO2.Challenge
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

### -Discoverable

Should this credential be discoverable.

```yaml
Type: System.Boolean
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

### -RelyingParty

RelaingParty object.

```yaml
Type: Yubico.YubiKey.Fido2.RelyingParty
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: UserData-RelyingParty
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: UserEntity-RelyingParty
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RelyingPartyID

Specify which relayingParty (site) this credential is regards to.

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: UserData-HostData
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: UserEntity-HostData
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RelyingPartyName

Friendlyname for the relayingParty.

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: UserData-HostData
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: UserEntity-HostData
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RequestedAlgorithms

Algorithms the RelyingParty accepts

```yaml
Type: System.Collections.Generic.List`1[Yubico.YubiKey.Fido2.Cose.CoseAlgorithmIdentifier]
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
- RS256
- ES512
- ES384
- ECDHwHKDF256
- EdDSA
- ES256
HelpMessage: ''
```

### -UserDisplayName

UserDisplayName to create credental for.

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: UserData-RelyingParty
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: UserData-HostData
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -UserEntity

Supply the user entity in complete form.

```yaml
Type: Yubico.YubiKey.Fido2.UserEntity
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: UserEntity-HostData
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: UserEntity-RelyingParty
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -UserID

UserID.

```yaml
Type: System.Byte[]
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: UserData-RelyingParty
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: UserData-HostData
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Username

Username to create credental for.

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: UserData-RelyingParty
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: UserData-HostData
  Position: Named
  IsRequired: true
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

- [Enroll YubiKey FIDO2 against demo.yubico.com](https://github.com/virot/powershellYK/blob/master/Docs/Examples/Enroll%20YubiKey%20FIDO2%20against%20demo.yubico.com.md)
- [Enroll YubiKey FIDO2 against login.microsoft.com](https://github.com/virot/powershellYK/blob/master/Docs/Examples/Enroll%20YubiKey%20FIDO2%20against%20login.microsoft.com.md)
