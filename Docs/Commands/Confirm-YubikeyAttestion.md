---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Confirm-YubikeyAttestion

## SYNOPSIS
Confirm YubiKey Attestion.

## SYNTAX

### requestWithExternalAttestion
```
Confirm-YubikeyAttestion -CertificateRequest <PSObject> -AttestionCertificate <PSObject>
 -IntermediateCertificate <PSObject> [<CommonParameters>]
```

### requestWithBuiltinAttestion
```
Confirm-YubikeyAttestion -CertificateRequest <PSObject> [<CommonParameters>]
```

### JustAttestCertificate
```
Confirm-YubikeyAttestion -AttestionCertificate <PSObject> -IntermediateCertificate <PSObject>
 [<CommonParameters>]
```

### CertificateIncludingAttestion
```
Confirm-YubikeyAttestion -CertificateIncludingAttestion <PSObject> [<CommonParameters>]
```

## DESCRIPTION
This cmdlet allows for verification of the attestion of YubiKeys. This can be used both to verify the attestion certificate and Certificate Request with and without built in attestion.

## EXAMPLES

### Example 1
```powershell
PS C:\> Confirm-YubikeyAttestion -CertificateRequest (New-YubikeyPIVCSR -Slot 0x9a -Attestation -PEMEncoded)

Slot                : 0x9A
AttestionValidated  : True
SerialNumber        : 12345678
FirmwareVersion     : 5.4.3
PinPolicy           : Once
TouchPolicy         : Never
FormFactor          : UsbAKeychain
isFIPSSeries        : False
isCSPNSeries        : False
AttestionMatchesCSR : True
```

Verify the certificate request created by New-YubikeyPIVCSR -Attestation.
Since this is a certificate request, *AttestionMatchesCSR* has a value.

### Example 2
```powershell
PS C:\> Confirm-YubikeyAttestion -AttestionCertificate (Assert-YubikeyPIV -Slot 0x9a) -IntermediateCertificate (Export-YubikeyPIVCertificate -AttestationIntermediateCertificate)

Slot                : 0x9A
AttestionValidated  : True
SerialNumber        : 12345678
FirmwareVersion     : 5.4.3
PinPolicy           : Once
TouchPolicy         : Never
FormFactor          : UsbAKeychain
isFIPSSeries        : False
isCSPNSeries        : False
AttestionMatchesCSR :
```

Verify the certificate request created by exported attestion and intermediate attestion certificates.
Since this did not include a Certificate Request, *AttestionMatchesCSR* is null.

### Example 3
 ```powershell
PS C:\> Confirm-YubikeyAttestion -CertificateRequest csr.pem -AttestionCertificate attestation.pem -IntermediateCertificate intermediate.pem

AttestionValidated  : True
SerialNumber        : 29167224
FirmwareVersion     : 5.7.1
PinPolicy           : Once
TouchPolicy         : Never
FormFactor          : UsbCKeychain
Slot                : 0x9A
Algorithm           : Rsa2048
isFIPSSeries        : False
isCSPNSeries        : False
AttestionMatchesCSR : True
```

Validate the certificate signing request (CSR) created _outside_ of **powershellYK**.
All three files should be provided as PEM files by the requesting party.

## PARAMETERS

### -AttestionCertificate
If the attestion certificate isn't include in the Certificate Request, please provide it here. The acceptable values for this parameter are:
- A file path
- A PEM string
- X509Certificate2

```yaml
Type: PSObject
Parameter Sets: requestWithExternalAttestion, JustAttestCertificate
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CertificateIncludingAttestion
CertificateIncludingAttestion

```yaml
Type: PSObject
Parameter Sets: CertificateIncludingAttestion
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
Parameter Sets: requestWithExternalAttestion, requestWithBuiltinAttestion
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -IntermediateCertificate
The intermediate certificate is signed by Yubico and unique in each YubiKey. If the intermediate attestion certificate isn't include in the Certificate Request, please provide it here. The acceptable values for this parameter are:
- A file path
- A PEM string
- X509Certificate2

```yaml
Type: PSObject
Parameter Sets: requestWithExternalAttestion, JustAttestCertificate
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
