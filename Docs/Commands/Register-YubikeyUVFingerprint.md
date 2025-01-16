---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Register-YubikeyUVFingerprint

## SYNOPSIS
Register a new fingerprint on a YubiKey Bio _or_ a YubiKey Bio Multi-Protocol Edition (MPE).

## SYNTAX

```
Register-YubikeyUVFingerprint [-Name <String>] [<CommonParameters>]
```

## DESCRIPTION
Register a new fingerprint on a YubiKey Bio _or_ a YubiKey Bio Multi-Protocol Edition (MPE).

## EXAMPLES

### Example 1
```powershell
PS C:\> Register-YubikeyUVFingerprint -Name "left index"

TemplateId                     FriendlyName
----------                     ------------
System.ReadOnlyMemory<Byte>[2] left index
```

This adds a new fingerprint to the YubiKey Bio / YubiKey Bio MPE.

## PARAMETERS

### -Name
Name of finger to register, for example: "left index" or "right index".

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
