---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: ''
Locale: en-SE
Module Name: powershellYK
ms.date: 03-20-2026
PlatyPS schema version: 2024-05-01
title: Export-YubiKeyFIDO2Blob
---

# Export-YubiKeyFIDO2Blob

## SYNOPSIS

Exports large blob from YubiKey FIDO2 by Credential ID or Relying Party ID (Origin).

## SYNTAX

### Export LargeBlob

```
Export-YubiKeyFIDO2Blob -CredentialId <CredentialID> -OutFile <FileInfo> [<CommonParameters>]
```

### Export LargeBlob by RelyingPartyID

```
Export-YubiKeyFIDO2Blob -RelyingPartyID <string> -OutFile <FileInfo> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Requires YubiKey firmware version 5.7.4 or later.

## EXAMPLES

### Example 1

```powershell
PS C:\> Export-YubiKeyFIDO2Blob -RelyingPartyID  "powershellYK" -OutFile storedfile.txt
Touch the YubiKey...
```

Exports the large blob for the credential with the specified Credential ID to the specified output file.

## PARAMETERS

### -CredentialId

Credential ID (hex or base64url string) to export large blob for.

```yaml
Type: System.Nullable`1[powershellYK.FIDO2.CredentialID]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Export LargeBlob
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -OutFile

Output file path for the exported large blob

```yaml
Type: System.IO.FileInfo
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Export LargeBlob
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: Export LargeBlob by RelyingPartyID
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

Relying Party ID (Origin), or relying party display name if unique, to export large blob for.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases:
- RP
- Origin
ParameterSets:
- Name: Export LargeBlob by RelyingPartyID
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

## OUTPUTS

### System.Object

{{ Fill in the Description }}

## NOTES

{{ Fill in the Notes }}

## RELATED LINKS

[FIDO2 large blobs ("largeBlobs" option)](https://docs.yubico.com/yesdk/users-manual/application-fido2/large-blobs.html)

