---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# New-YubikeyPIVCSR

## SYNOPSIS
Creates a CSR for a slot in the Yubikey

## SYNTAX

```
New-YubikeyPIVCSR -Slot <Byte> [-Attestation] [-Subjectname <String>] [-OutFile <String>]
 [-HashAlgorithm <HashAlgorithmName>] [-PEMEncoded] [<CommonParameters>]
```

## DESCRIPTION
Cmdlet that allows the creating of CSR to send to a CA. This allows the configuration of what the CSR should contain.

## EXAMPLES

### Example 1
```powershell
PS C:\> $CSR = New-YubikeyPIVCSR -Slot 0x9a -Subjectname 'CN=User,O=Company,C=SE'
```

Would create a CSR with the Subjectname "CN=User,O=Company,C=SE" and store it in the variable $CSR.

### Example 2
```powershell
PS C:\> New-YubikeyPIVCSR -Slot 0x9a -OutFile "$($env:TEMP)\certificate_request.req"
```

Would create a CSR with the default Subjectname and store it as certificate_request.req in the temp folder.

### Example 3
```powershell
PS C:\> $CSR = New-YubikeyPIVCSR -Slot 0x9a -Attestation
```

Would create a CSR with attestation included and store it in the variable $CSR

## PARAMETERS

### -Attestation
Include attestion certificate in CSR

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

### -Slot
Create a CSR for slot

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
