---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Get-YubiKeyFIDO2Credential

## SYNOPSIS

Read the FIDO2 discoverable credentials

## SYNTAX

### List-All (Default)

```
Get-YubiKeyFIDO2Credential [-All] [<CommonParameters>]
```

### List-CredentialID

```
Get-YubiKeyFIDO2Credential -CredentialID <CredentialID> [<CommonParameters>]
```

### List-CredentialID-Base64URL

```
Get-YubiKeyFIDO2Credential -CredentialIdBase64Url <String> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Get what FIDO2 credentials that have been saved in the Yubikey.

## EXAMPLES

### Example 1

```powershell
PS C:\> Get-YubikeyFIDO2Credential

Site            Name         DisplayName
----            ----         -----------
demo.yubico.com powershellYK powershellYK
```

Lists all sites and usernames for all discoverable credentials.

## PARAMETERS

### -All

List all

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: List-All
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CredentialID

Credential ID to remove

```yaml
Type: System.Nullable`1[powershellYK.FIDO2.CredentialID]
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: List-CredentialID
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CredentialIdBase64Url

Credential ID to remove int Base64 URL encoded format

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: List-CredentialID-Base64URL
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

