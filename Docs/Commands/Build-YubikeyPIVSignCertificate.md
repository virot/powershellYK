---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Build-YubikeyPIVSignCertificate

## SYNOPSIS
Sign a certificate request with a Yubikey

## SYNTAX

```
Build-YubikeyPIVSignCertificate -CertificateRequest <PSObject> -Slot <Byte>
 [-HashAlgorithm <HashAlgorithmName>] [-OutFile <String>] [-PEMEncoded] [-Subjectname <String>]
 [-NotBefore <DateTimeOffset>] [-NotAfter <DateTimeOffset>] [-SerialNumber <Byte[]>] [-CertificateAuthority]
 [-SubjectAltName <String[]>] [-KeyUsage <X509KeyUsageFlags>] [<CommonParameters>]
```

## DESCRIPTION
Allows the signing of a certificate request with a Yubikey. The certificate request must be in the form of a CSR in PEM format with the following properties:

## EXAMPLES

### Example 1
```powershell
PS C:\> Build-YubikeyPIVSignCertificate -CertificateRequest "C:\temp\input.csr" -Slot 0x9d -Subjectname "CN=Signed site" -SubjectAltName ("DNS siteurl","DNS second.url") -OutFile "C:\temp\server.cer"
```

Sign a certificate request with a new Subhectname and alternative names.
The certificate will contain the Subjectname "CN=Signed site" and the alternative names "siteurl" and "second.url".

## PARAMETERS

### -CertificateAuthority
Make this a CA certificate

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CertificateRequest
Certificate request

```yaml
Type: PSObject
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -HashAlgorithm
HashAlgoritm

```yaml
Type: HashAlgorithmName
Parameter Sets: (All)
Aliases:
Accepted values: SHA1, SHA256, SHA384, SHA512

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -KeyUsage
Key usage options to include

```yaml
Type: X509KeyUsageFlags
Parameter Sets: (All)
Aliases:
Accepted values: None, EncipherOnly, CrlSign, KeyCertSign, KeyAgreement, DataEncipherment, KeyEncipherment, NonRepudiation, DigitalSignature, DecipherOnly

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NotAfter
Certificate to be valid until

```yaml
Type: DateTimeOffset
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NotBefore
Certificate to be valid from

```yaml
Type: DateTimeOffset
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutFile
Output file

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PEMEncoded
Encode output as PEM

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SerialNumber
Serial number for certificate

```yaml
Type: Byte[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Slot
Slot to sign certificate with

```yaml
Type: Byte
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SubjectAltName
SubjectAlternativeNames for the certificate
Start each string with DNS, MAIL or UPN and a space before the value.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Subjectname
Subjectname of certificate

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
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
