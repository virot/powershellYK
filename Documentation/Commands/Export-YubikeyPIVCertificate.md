---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Export-YubikeyPIVCertificate

## SYNOPSIS
Export certificate from Yubikey PIV

## SYNTAX

### Slot
```
Export-YubikeyPIVCertificate -Slot <Byte> [-OutFile <String>] [-PEMEncoded]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### AttestationCertificate
```
Export-YubikeyPIVCertificate [-AttestationIntermediateCertificate] [-OutFile <String>] [-PEMEncoded]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Export certificates from Yubikey

## EXAMPLES

### Example 1
```powershell
PS C:\> $Certificate = Export-YubikeyPIVCertificate -Slot 0x9a
```

Exports the certificate to a variable for futher processing.

### Example 2
```powershell
PS C:\> Export-YubikeyPIVCertificate -Slot 0x9a -OutFile "$($env:TEMP)\exported_certificate.cer"
```

Exports the certificate from slot 0x9a and stores it as exported_certificate.cer in the temp folder.

### Example 3
```powershell
PS C:\> Export-YubikeyPIVCertificate -AttestationCertificate -OutFile yubikey_intermediate_attestation.cer
```

Exports the builtin intermediate attestation certificate.

## PARAMETERS

### -AttestationIntermediateCertificate
Export Attestation certificate

```yaml
Type: SwitchParameter
Parameter Sets: AttestationCertificate
Aliases: AttestationCertificate

Required: True
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

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Slot
Slot to extract

```yaml
Type: Byte
Parameter Sets: Slot
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
