---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Remove-YubikeyFIDO2Credential

## SYNOPSIS

Removes a FIDO2 credential from the YubiKey.

## SYNTAX

### Remove with CredentialID (Default)

```
Remove-YubikeyFIDO2Credential -CredentialId <CredentialID> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Remove with username and RelayingParty

```
Remove-YubikeyFIDO2Credential -Username <String> -RelayingParty <String> [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Allows the removal of a FIDO2 credential from the YubiKey. The credential can be removed by specifying the CredentialID or by specifying the Username and RelayingParty.
The Cmdlet also allows piping of the CredentialID to remove the credential.

## EXAMPLES

### Example 1

```powershell
PS C:\> Remove-YubikeyFIDO2Credential -User 'powershellYK' -RelayingParty 'demo.yubico.com'
```

Removes the credential for the user 'powershellYK' from the RelayingParty 'demo.yubico.com'

### Example 2

```powershell
PS C:\> Remove-YubikeyFIDO2Credential -CredentialId ac37c06c15ec4458d0cf545db3cc0f8e3992e512d1c3e19d571417b12124634f01e6e3397bdbc8e74b96f950ea4bf600
```

Removes the credential with a specified CredentialID

### Example 3

```powershell
PS C:\> Get-YubiKeyFIDO2Credential|Where-Object RPId -eq 'demo.yubico.com'|Remove-YubikeyFIDO2Credential -Confirm:$false
```

Removes all FIDO2 credentials for the RelayingParty 'demo.yubico.com'

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

### -CredentialId

Credential ID to remove

```yaml
Type: powershellYK.FIDO2.CredentialID
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Remove with CredentialID
  Position: Named
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RelayingParty

RelayingParty to remove user from

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Remove with username and RelayingParty
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

User to remove

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Remove with username and RelayingParty
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

### powershellYK.FIDO2.CredentialID

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

