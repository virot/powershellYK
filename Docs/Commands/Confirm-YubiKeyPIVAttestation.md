---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Confirm-YubiKeyPIVAttestation

## SYNOPSIS

Confirm YubiKey Attestation.

## SYNTAX

### requestWithExternalAttestation-Object

```
Confirm-YubiKeyPIVAttestation -CertificateRequest <CertificateRequest>
 -AttestationCertificate <X509Certificate2> -IntermediateCertificate <X509Certificate2>
 [<CommonParameters>]
```

### requestWithBuiltinAttestation-Object

```
Confirm-YubiKeyPIVAttestation -CertificateRequest <CertificateRequest> [<CommonParameters>]
```

### requestWithExternalAttestation-File

```
Confirm-YubiKeyPIVAttestation -CertificateRequestFile <FileInfo>
 -AttestationCertificateFile <FileInfo> -IntermediateCertificateFile <FileInfo> [<CommonParameters>]
```

### requestWithBuiltinAttestation-File

```
Confirm-YubiKeyPIVAttestation -CertificateRequestFile <FileInfo> [<CommonParameters>]
```

### JustAttestCertificate-Object

```
Confirm-YubiKeyPIVAttestation -AttestationCertificate <X509Certificate2>
 -IntermediateCertificate <X509Certificate2> [<CommonParameters>]
```

### JustAttestCertificate-File

```
Confirm-YubiKeyPIVAttestation -AttestationCertificateFile <FileInfo>
 -IntermediateCertificateFile <FileInfo> [<CommonParameters>]
```

### CertificateIncludingAttestation-Object

```
Confirm-YubiKeyPIVAttestation -CertificateIncludingAttestation <X509Certificate2>
 [<CommonParameters>]
```

### CertificateIncludingAttestation-File

```
Confirm-YubiKeyPIVAttestation -CertificateIncludingAttestationFile <FileInfo> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

This cmdlet allows for verification of the attestation of YubiKeys. This can be used both to verify the attestation certificate and Certificate Request with and without built in attestation.

## EXAMPLES

### Example 1

```powershell
PS C:\> New-YubikeyPIVCSR -Slot 0x9a -Attestation -OutFile csr.pem
PS C:\> Confirm-YubiKeyPIVAttestation -CertificateRequestFile csr.pem

Slot                : 0x9A
AttestationValidated  : True
SerialNumber        : 12345678
FirmwareVersion     : 5.4.3
PinPolicy           : Once
TouchPolicy         : Never
FormFactor          : UsbAKeychain
isFIPSSeries        : False
isCSPNSeries        : False
AttestationMatchesCSR : True
```

Verify the certificate request created by New-YubikeyPIVCSR -Attestation.
Since this is a certificate request, *AttestationMatchesCSR* has a value.

### Example 2

```powershell
PS C:\> Confirm-YubiKeyPIVAttestation -AttestationCertificate (Assert-YubikeyPIV -Slot 0x9a) -IntermediateCertificate (Export-YubikeyPIVCertificate -AttestationIntermediateCertificate)

Slot                : 0x9A
AttestationValidated  : True
SerialNumber        : 12345678
FirmwareVersion     : 5.4.3
PinPolicy           : Once
TouchPolicy         : Never
FormFactor          : UsbAKeychain
isFIPSSeries        : False
isCSPNSeries        : False
AttestationMatchesCSR :
```

Verify the certificate request created by exported attestation and intermediate attestation certificates.
Since this did not include a Certificate Request, *AttestationMatchesCSR* is null.

### Example 3

```powershell
PS C:\> Confirm-YubiKeyPIVAttestation -CertificateRequestFile csr.pem -AttestationCertificateFile attestation.pem -IntermediateCertificateFile intermediate.pem

AttestationValidated  : True
SerialNumber        : 29167224
FirmwareVersion     : 5.7.1
PinPolicy           : Once
TouchPolicy         : Never
FormFactor          : UsbCKeychain
Slot                : 0x9A
Algorithm           : Rsa2048
isFIPSSeries        : False
isCSPNSeries        : False
AttestationMatchesCSR : True
```

Validate the certificate signing request (CSR) created _outside_ of **powershellYK**.
All three files should be provided as PEM files by the requesting party.

## PARAMETERS

### -AttestationCertificate

AttestationCertificate

```yaml
Type: System.Security.Cryptography.X509Certificates.X509Certificate2
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: requestWithExternalAttestation-Object
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: JustAttestCertificate-Object
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -AttestationCertificateFile

AttestationCertificate

```yaml
Type: System.IO.FileInfo
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: requestWithExternalAttestation-File
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: JustAttestCertificate-File
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CertificateIncludingAttestation

CertificateIncludingAttestation

```yaml
Type: System.Security.Cryptography.X509Certificates.X509Certificate2
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: CertificateIncludingAttestation-Object
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CertificateIncludingAttestationFile

CertificateIncludingAttestation

```yaml
Type: System.IO.FileInfo
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: CertificateIncludingAttestation-File
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CertificateRequest

CSR to check

```yaml
Type: System.Security.Cryptography.X509Certificates.CertificateRequest
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: requestWithExternalAttestation-Object
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: requestWithBuiltinAttestation-Object
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CertificateRequestFile

CSR to check

```yaml
Type: System.IO.FileInfo
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: requestWithExternalAttestation-File
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: requestWithBuiltinAttestation-File
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -IntermediateCertificate

IntermediateCertificate

```yaml
Type: System.Security.Cryptography.X509Certificates.X509Certificate2
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: requestWithExternalAttestation-Object
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: JustAttestCertificate-Object
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -IntermediateCertificateFile

IntermediateCertificate

```yaml
Type: System.IO.FileInfo
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: requestWithExternalAttestation-File
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: JustAttestCertificate-File
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

