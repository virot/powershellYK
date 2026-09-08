---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: ''
Locale: en-SE
Module Name: powershellYK
PlatyPS schema version: 2024-05-01
title: Protect-YubiKeyFIDO2File
---

# Protect-YubiKeyFIDO2File

## SYNOPSIS

Encrypts a file using FIDO2 PRF (hmac-secret / hmac-secret-mc) extension on a YubiKey.

## SYNTAX

### AutoCreate (Default)

```
Protect-YubiKeyFIDO2File -Path <FileInfo> [-OutFile <FileInfo>] [-Force] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### WithCredential

```
Protect-YubiKeyFIDO2File -Path <FileInfo> -Credential <Credential> [-OutFile <FileInfo>] [-Force]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### WithCredentialID

```
Protect-YubiKeyFIDO2File -Path <FileInfo> -CredentialID <CredentialID> -RelyingPartyID <string>
 [-OutFile <FileInfo>] [-Force] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### ByRelyingPartyID

```
Protect-YubiKeyFIDO2File -Path <FileInfo> -RelyingPartyID <string> [-OutFile <FileInfo>] [-Force]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Encrypts a file using FIDO2 PRF (hmac-secret) extension on a YubiKey.
Uses HKDF-SHA256 for key derivation and AES-256-GCM for authenticated encryption.
Requires a YubiKey with FIDO2 hmac-secret support and administrator privileges on Windows.

When no credential or relying party is provided, a synthetic `prf-encryption` credential is created or reused. On firmware 5.8+ (`hmac-secret-mc`), that first-time AutoCreate completes in one touch: the PRF is returned during credential creation. Older firmware still enables `hmac-secret` at creation and then requests the PRF with a second `GetAssertions` call (two touches). Reusing an existing credential, passing `-Credential`, and decrypting with `Unprotect-YubiKeyFIDO2File` always use a single assertion.

## EXAMPLES

### Example 1

```powershell
Protect-YubiKeyFIDO2File -Path .\secret.txt -Force
```
Encrypts secret.txt, automatically creating or reusing a `prf-encryption` credential. On firmware 5.8+ the first-time create+encrypt is a single touch.

### Example 2

```powershell
$cred = Get-YubiKeyFIDO2Credential | Where-Object { $_.RelyingParty.Id -eq "demo.yubico.com" }
Protect-YubiKeyFIDO2File -Path .\secret.txt -Credential $cred
```
Encrypts secret.txt using the specified FIDO2 credential

### Example 3

```powershell
Get-Item .\secret.txt | Protect-YubiKeyFIDO2File -Credential $cred
```
Encrypts a file via pipeline input

### Example 4

```powershell
Protect-YubiKeyFIDO2File -Path .\secret.txt -RelyingPartyID "demo.yubico.com"
```
Encrypts using the sole credential for that relying party (aliases -RP and -Origin).

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

### -Credential

Credential object from Get-YubiKeyFIDO2Credential.

```yaml
Type: powershellYK.FIDO2.Credential
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: WithCredential
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CredentialID

Credential ID to use.

```yaml
Type: powershellYK.FIDO2.CredentialID
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: WithCredentialID
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

Suppress the confirmation prompt when auto-creating a `prf-encryption` credential.
Suppress the confirmation prompt when auto-creating a credential.
Suppress the confirmation prompt when auto-creating a credential.
Suppress the confirmation prompt when auto-creating a credential.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
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

### -OutFile

Output file path. Defaults to input path with .enc extension.

```yaml
Type: System.IO.FileInfo
DefaultValue: ''
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

### -Path

Input file to encrypt.

```yaml
Type: System.IO.FileInfo
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RelyingPartyID

Relying Party ID for the assertion.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases:
- RP
- Origin
ParameterSets:
- Name: WithCredentialID
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ByRelyingPartyID
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

### System.IO.FileInfo

{{ Fill in the Description }}

### powershellYK.FIDO2.Credential

{{ Fill in the Description }}

## OUTPUTS

### System.Object

{{ Fill in the Description }}

## NOTES

FIDO private keys are non-exportable and never leave the YubiKey. Decryption therefore depends entirely on continued access to the original YubiKey _and_ the specific credential used for encryption. If the YubiKey is lost, reset, or if the credential is removed, the encrypted data cannot be recovered.

On firmware 5.8+ (`hmac-secret-mc`), first-time AutoCreate encryption requires one user interaction. Older firmware requires two (create credential, then assert). Decrypt and encrypt-with-existing-credential always require one assertion.

## RELATED LINKS

{{ Fill in the related links here }}

