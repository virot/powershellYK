---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Import-YubikeyPIVCertificate

## SYNOPSIS
Import certificate

## SYNTAX

```
Import-YubikeyPIVCertificate -Slot <Byte> -Certificate <Object> [<CommonParameters>]
```

## DESCRIPTION
Imports a certicate into the Yubikey

## EXAMPLES

### Example 1
```powershell
PS C:\> Import-YubikeyPIVCertificate -Slot 0x9a -Path certificate.cer
```

Import certificate.cer into the certificate slot 0x9a

## PARAMETERS

### -Certificate
Certificate to be stored

```yaml
Type: Object
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Slot
Slotnumber

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
