---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Confirm-YubiKeyPIVAttestation

## SYNOPSIS
Confirm YubiKey Attestation.

## SYNTAX

### requestWithExternalAttestation-Object
```
Confirm-YubiKeyPIVAttestation -CertificateRequest <CertificateRequest>
 -AttestationCertificate <X509Certificate2> -IntermediateCertificate <X509Certificate2> [<CommonParameters>]
```

### requestWithBuiltinAttestation-Object
```
Confirm-YubiKeyPIVAttestation -CertificateRequest <CertificateRequest> [<CommonParameters>]
```

### requestWithExternalAttestation-File
```
Confirm-YubiKeyPIVAttestation -CertificateRequestFile <FileInfo> -AttestationCertificateFile <FileInfo>
 -IntermediateCertificateFile <FileInfo> [<CommonParameters>]
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
Confirm-YubiKeyPIVAttestation -AttestationCertificateFile <FileInfo> -IntermediateCertificateFile <FileInfo>
 [<CommonParameters>]
```

### CertificateIncludingAttestation-Object
```
Confirm-YubiKeyPIVAttestation -CertificateIncludingAttestation <X509Certificate2> [<CommonParameters>]
```

### CertificateIncludingAttestation-File
```
Confirm-YubiKeyPIVAttestation -CertificateIncludingAttestationFile <FileInfo> [<CommonParameters>]
```

## DESCRIPTION
This cmdlet allows for verification of the attestation of YubiKeys. This can be used both to verify the attestation certificate and Certificate Request with and without built in attestation.

## EXAMPLES

### Example 1
```powershell
PS C:\> Confirm-YubiKeyPIVAttestation -CertificateRequest (New-YubikeyPIVCSR -Slot 0x9a -Attestation -PEMEncoded)

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
PS C:\> Confirm-YubiKeyPIVAttestation -CertificateRequest csr.pem -AttestationCertificate attestation.pem -IntermediateCertificate intermediate.pem

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
Type: X509Certificate2
Parameter Sets: requestWithExternalAttestation-Object, JustAttestCertificate-Object
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AttestationCertificateFile
AttestationCertificate

```yaml
Type: FileInfo
Parameter Sets: requestWithExternalAttestation-File, JustAttestCertificate-File
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CertificateIncludingAttestation
CertificateIncludingAttestation

```yaml
Type: X509Certificate2
Parameter Sets: CertificateIncludingAttestation-Object
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CertificateIncludingAttestationFile
CertificateIncludingAttestation

```yaml
Type: FileInfo
Parameter Sets: CertificateIncludingAttestation-File
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CertificateRequest
CSR to check

```yaml
Type: CertificateRequest
Parameter Sets: requestWithExternalAttestation-Object, requestWithBuiltinAttestation-Object
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CertificateRequestFile
CSR to check

```yaml
Type: FileInfo
Parameter Sets: requestWithExternalAttestation-File, requestWithBuiltinAttestation-File
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -IntermediateCertificate
IntermediateCertificate

```yaml
Type: X509Certificate2
Parameter Sets: requestWithExternalAttestation-Object, JustAttestCertificate-Object
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -IntermediateCertificateFile
IntermediateCertificate

```yaml
Type: FileInfo
Parameter Sets: requestWithExternalAttestation-File, JustAttestCertificate-File
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
