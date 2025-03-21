﻿---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Build-YubiKeyPIVCertificateSigningRequest

## SYNOPSIS
Creates a CSR for a slot in the YubiKey.

## SYNTAX

### With Attestation
```
Build-YubiKeyPIVCertificateSigningRequest -Slot <PIVSlot> [-Attestation] [-AttestationLocation <String>]
 [-Subjectname <String>] [-OutFile <FileInfo>] [-HashAlgorithm <HashAlgorithmName>] [-PEMEncoded]
 [<CommonParameters>]
```

### Without Attestation
```
Build-YubiKeyPIVCertificateSigningRequest -Slot <PIVSlot> [-Subjectname <String>] [-OutFile <FileInfo>]
 [-HashAlgorithm <HashAlgorithmName>] [-PEMEncoded] [<CommonParameters>]
```

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
Include attestion certificate in CSR

```yaml
Type: SwitchParameter
Parameter Sets: With Attestation
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AttestationLocation
OID to store attestation in CSR
Legacy stores the attestation in the .11 OID as yubico-piv-tool used until 2025.
Standard stores the attestation in the .1 OID as yubico-piv-tool uses from 2025.
Both stores the attestation in both OIDs.

```yaml
Type: String
Parameter Sets: With Attestation
Aliases:
Accepted values: Both, Legacy, Standard

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -HashAlgorithm
HashAlgoritm, this will be forced to correct for ECC.

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

### -OutFile
Save CSR as file

```yaml
Type: FileInfo
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

### -Slot
Create a CSR for slot

```yaml
Type: PIVSlot
Parameter Sets: (All)
Aliases:

Required: True
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
