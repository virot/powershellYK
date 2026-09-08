---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: ''
Locale: en-SE
Module Name: powershellYK
PlatyPS schema version: 2024-05-01
title: Import-YubiKeyFIDO2Blob
---

# Import-YubiKeyFIDO2Blob

## SYNOPSIS

Imports large blob to YubiKey FIDO2 by Credential ID or Relying Party ID (Origin).

## SYNTAX

### Set LargeBlob

```
Import-YubiKeyFIDO2Blob -Path <FileInfo> -CredentialId <CredentialID> [-Force] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### Set LargeBlob by RelyingPartyID

```
Import-YubiKeyFIDO2Blob -Path <FileInfo> -RelyingPartyID <string> [-Force] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### AutoCreate (Default)

```
Import-YubiKeyFIDO2Blob -Path <FileInfo> [-Force] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Requires YubiKey firmware version 5.7 or later.

## EXAMPLES

### Example 1

```powershell
PS C:\> Import-YubiKeyFIDO2Blob -RelyingPartyID "powershellYK" -LargeBlob FileToImport.txt
Touch the YubiKey...
```

Imports the large blob from the specified file for the credential with the specified Relying Party ID (or display name, if unique) to the YubiKey.

## PARAMETERS

### -Confirm

Prompts you for confirmation before running the cmdlet.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
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

Credential ID (hex or base64url string) to associate with the large blob array.

```yaml
Type: System.Nullable`1[powershellYK.FIDO2.CredentialID]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Set LargeBlob
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Force

Overwrite existing large blob entry for this credential without prompting.
Suppress confirmation prompts (credential creation and blob overwrite).

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: AutoCreate
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: Set LargeBlob
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: Set LargeBlob by RelyingPartyID
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -LargeBlob

File to import as large blob

```yaml
Type: System.IO.FileInfo
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Set LargeBlob
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: Set LargeBlob by RelyingPartyID
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Path

File to import as large blob

```yaml
Type: System.IO.FileInfo
DefaultValue: ''
SupportsWildcards: false
Aliases:
- LargeBlob
- File
ParameterSets:
- Name: AutoCreate
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: Set LargeBlob
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: Set LargeBlob by RelyingPartyID
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

Relying party ID, or relying party display name if unique, to associate with the large blob.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases:
- RP
- Origin
ParameterSets:
- Name: Set LargeBlob by RelyingPartyID
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
DefaultValue: ''
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

## OUTPUTS

### System.Object

{{ Fill in the Description }}

## NOTES

{{ Fill in the Notes }}

## RELATED LINKS

- [FIDO2 large blobs ("largeBlobs" option)](https://docs.yubico.com/yesdk/users-manual/application-fido2/large-blobs.html)
