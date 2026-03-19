---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Build-YubiKeyPIVCertificateSigningRequest

## SYNOPSIS

Creates a CSR for a slot in the YubiKey.

## SYNTAX

### With Attestation

```
Build-YubiKeyPIVCertificateSigningRequest -Slot <PIVSlot> -Attestation
 [-AttestationLocation <string>] [-Subjectname <string>] [-OutFile <FileInfo>]
 [-HashAlgorithm <HashAlgorithmName>] [-PEMEncoded] [<CommonParameters>]
```

### Without Attestation

```
Build-YubiKeyPIVCertificateSigningRequest -Slot <PIVSlot> [-Subjectname <String>]
 [-OutFile <FileInfo>] [-HashAlgorithm <HashAlgorithmName>] [-PEMEncoded] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Cmdlet that allows the creating of CSR to send to a CA. This allows the configuration of what the CSR should contain.

## EXAMPLES

### Example 1

```powershell
PS C:\> $CSR = Build-YubiKeyPIVCertificateSigningRequest -Slot 0x9a -Subjectname 'CN=User,O=Company,C=SE'
```

Would create a CSR with the Subjectname "CN=User,O=Company,C=SE" and store it in the variable $CSR.

### Example 2

```powershell
PS C:\> Build-YubiKeyPIVCertificateSigningRequest -Slot 0x9a -OutFile "$($env:TEMP)\certificate_request.req"
```

Would create a CSR with the default Subjectname and store it as certificate_request.req in the temp folder.

### Example 3

```powershell
PS C:\> $CSR = Build-YubiKeyPIVCertificateSigningRequest -Slot 0x9a -Attestation -PEMEncoded
```

Would create a CSR with attestation included and store it in the variable $CSR

## PARAMETERS

### -Attestation

Include attestation certificate in CSR

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: With Attestation
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -AttestationLocation

OID location to store attestation in CSR.
Legacy stores the attestation in the .11 OID as yubico-piv-tool used until 2025.
Standard stores the attestation in the .1 OID as yubico-piv-tool uses from 2025.
Both stores the attestation in both OIDs.

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: With Attestation
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- Both
- Legacy
- Standard
HelpMessage: ''
```

### -HashAlgorithm

HashAlgoritm, this will be forced to correct for ECC.

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

### -OutFile

Save CSR as file

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

### -Slot

Create a CSR for slot

```yaml
Type: powershellYK.PIV.PIVSlot
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: With Attestation
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: Without Attestation
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Subjectname

Subjectname of certificate

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

