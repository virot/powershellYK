# Generate YubiKey attestation and verify

The [powershellYK](https://github.com/virot/powershellYK) powershell7 module allows generation of YubiKey attestation.
YubiKey attestation allows the proving that the private key resides and was generated on a geniuine YubiKey. This can also be bundled in the Certificate Signing Request to allow the Certificate Authority to verify the YubiKey before approving the request.

The YubiKey contains a manufacture time private key and certificate signed by a private Yubico Certificate Authority.
The intermediate attestation certificate is per YubiKey.

## Generate attestion certificate and intermediate certificate
```pwsh
Assert-YubikeyPIV -Slot "PIV Authentication" -OutFile attestation.cer
Export-YubikeyPIVCertificate -AttestationIntermediateCertificate -OutFile intermediate.cer
```

## Generate a Certificate Request with included attestion
```pwsh
Build-YubiKeyPIVCertificateSigningRequest -Slot "PIV Authentication" -Subjectname "CN=powershellYK" -Attestation -OutFile attested.csr -PEMEncoded
```

## Validate that a submitted attestion & intermediate certificates
```pwsh
Confirm-YubikeyAttestion -AttestionCertificate attestation.cer -IntermediateCertificate intermediate.cer
```

## Validate Certificate Request with included attestion
```pwsh
Confirm-YubikeyAttestion -CertificateRequest attested.csr
```