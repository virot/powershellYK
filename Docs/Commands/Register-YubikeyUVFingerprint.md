---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Register-YubikeyUVFingerprint

## SYNOPSIS
Register a new fingerprint on the YubiKey Bio.

## SYNTAX

```
Register-YubikeyUVFingerprint [-Name <String>] [<CommonParameters>]
```

## DESCRIPTION
Register a new fingerprint on the yubikey Bio.

## EXAMPLES

### Example 1
```powershell
PS C:\> Register-YubikeyUVFingerprint -Name "tumme"

TemplateId                     FriendlyName
----------                     ------------
System.ReadOnlyMemory<Byte>[2] tumme
```

This adds a new fingerprint to the yubikey Bio.

## PARAMETERS

### -Name
Name of finger to register

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
