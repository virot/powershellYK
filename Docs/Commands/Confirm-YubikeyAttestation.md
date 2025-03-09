---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Confirm-YubiKeyAttestation

## SYNOPSIS
Confirm YubiKey Attestation.

## SYNTAX

### requestWithExternalAttestation
```
Confirm-YubiKeyAttestation -CertificateRequest <PSObject> -AttestationCertificate <PSObject>
 -IntermediateCertificate <PSObject> [<CommonParameters>]
```

### requestWithBuiltinAttestation
```
Confirm-YubiKeyAttestation -CertificateRequest <PSObject> [<CommonParameters>]
```

### JustAttestCertificate
```
Confirm-YubiKeyAttestation -AttestationCertificate <PSObject> -IntermediateCertificate <PSObject>
 [<CommonParameters>]
```

### CertificateIncludingAttestation
```
Confirm-YubiKeyAttestation -CertificateIncludingAttestation <PSObject> [<CommonParameters>]
```

## DESCRIPTION
This cmdlet allows for verification of the attestation of YubiKeys. This can be used both to verify the attestation certificate and Certificate Request with and without built in attestation.

## EXAMPLES

### Example 1
```powershell
PS C:\> Confirm-YubikeyAttestation -CertificateRequest (New-YubikeyPIVCSR -Slot 0x9a -Attestation -PEMEncoded)

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
PS C:\> Confirm-YubikeyAttestation -AttestationCertificate (Assert-YubikeyPIV -Slot 0x9a) -IntermediateCertificate (Export-YubikeyPIVCertificate -AttestationIntermediateCertificate)

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
PS C:\> Confirm-YubikeyAttestation -CertificateRequest csr.pem -AttestationCertificate attestation.pem -IntermediateCertificate intermediate.pem

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
If the attestation certificate isn't include in the Certificate Request, please provide it here. The acceptable values for this parameter are:
- A file path
- A PEM string
- X509Certificate2

```yaml
Type: PSObject
Parameter Sets: requestWithExternalAttestation, JustAttestCertificate
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
Type: PSObject
Parameter Sets: CertificateIncludingAttestation
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CertificateRequest
Specifies a CertificateRequest to check. The acceptable values for this parameter are:
- A file path
- A PEM string
- A byte array (byte[])

```yaml
Type: PSObject
Parameter Sets: requestWithExternalAttestation, requestWithBuiltinAttestation
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -IntermediateCertificate
The intermediate certificate is signed by Yubico and unique in each YubiKey. If the intermediate attestation certificate isn't include in the Certificate Request, please provide it here. The acceptable values for this parameter are:
- A file path
- A PEM string
- X509Certificate2

```yaml
Type: PSObject
Parameter Sets: requestWithExternalAttestation, JustAttestCertificate
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
