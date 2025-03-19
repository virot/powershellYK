---
external help file: powershellYK.dll-help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Confirm-YubiKeyAttestation

## SYNOPSIS
Confirm YubiKey Attestation.

## SYNTAX

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
