---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Build-YubiKeyPIVSignCertificate

## SYNOPSIS

Sign a certificate request with a YubiKey.

## SYNTAX

### Default (Default)

```
Build-YubiKeyPIVSignCertificate -CertificateRequest <PSObject> -Slot <PIVSlot>
 [-HashAlgorithm <HashAlgorithmName>] [-OutFile <FileInfo>] [-PEMEncoded] [-Subjectname <String>]
 [-NotBefore <DateTimeOffset>] [-NotAfter <DateTimeOffset>] [-SerialNumber <Byte[]>]
 [-CertificateAuthority] [-SubjectAltName <String[]>] [-KeyUsage <X509KeyUsageFlags>]
 [-AIAUrl <String>] [<CommonParameters>]
```

### __AllParameterSets

```
Build-YubiKeyPIVSignCertificate -CertificateRequest <psobject> -Slot <PIVSlot>
 [-HashAlgorithm <HashAlgorithmName>] [-OutFile <FileInfo>] [-PEMEncoded] [-SKI <string>]
 [-Subjectname <string>] [-NotBefore <DateTimeOffset>] [-NotAfter <DateTimeOffset>]
 [-SerialNumber <byte[]>] [-CertificateAuthority] [-SubjectAltName <string[]>]
 [-KeyUsage <X509KeyUsageFlags>] [-AIAUrl <string>] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Allows the signing of a certificate request with a YubiKey. The certificate request must be in the form of a CSR in PEM format with the following properties:

## EXAMPLES

### Example 1

```powershell
PS C:\> Build-YubikeyPIVSignCertificate -CertificateRequest "C:\temp\input.csr" -Slot 0x9d -Subjectname "CN=Signed site" -SubjectAltName ("DNS siteurl","DNS second.url") -OutFile "C:\temp\server.cer"
```

Sign a certificate request with a new Subhectname and alternative names.
The certificate will contain the Subjectname "CN=Signed site" and the alternative names "siteurl" and "second.url".

## PARAMETERS

### -AIAUrl

AIA URL to include in signed certificates

```yaml
Type: System.String
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

### -CertificateAuthority

Make this a CA certificate

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

### -CertificateRequest

Certificate request

```yaml
Type: System.Management.Automation.PSObject
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

### -HashAlgorithm

HashAlgoritm

```yaml
Type: System.Security.Cryptography.HashAlgorithmName
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
- SHA1
- SHA256
- SHA384
- SHA512
HelpMessage: ''
```

### -KeyUsage

Key usage options to include

```yaml
Type: System.Security.Cryptography.X509Certificates.X509KeyUsageFlags
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
- EncipherOnly
- CrlSign
- KeyCertSign
- KeyAgreement
- DataEncipherment
- KeyEncipherment
- NonRepudiation
- DigitalSignature
- DecipherOnly
HelpMessage: ''
```

### -NotAfter

Certificate to be valid until

```yaml
Type: System.DateTimeOffset
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

### -NotBefore

Certificate to be valid from

```yaml
Type: System.DateTimeOffset
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

### -OutFile

Output file

```yaml
Type: System.IO.FileInfo
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

### -PEMEncoded

Encode output as PEM

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

### -SerialNumber

Serial number for certificate

```yaml
Type: System.Byte[]
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

### -SKI

Custom SKI for debugging

```yaml
Type: System.String
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

### -Slot

Slot to sign certificate with

```yaml
Type: powershellYK.PIV.PIVSlot
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

### -SubjectAltName

SubjectAlternativeNames for the certificate
Start each string with DNS, MAIL or UPN and a space before the value.

```yaml
Type: System.String[]
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

### -Subjectname

Subject name of certificate

```yaml
Type: System.String
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

