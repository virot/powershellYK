---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Find-Yubikey

## SYNOPSIS
Lists all Yubikeys on system

## SYNTAX

```
Find-Yubikey [-OnlyOne] [-Serialnumber <Int32>] [<CommonParameters>]
```

## DESCRIPTION
List all Yubikeys on the system

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-Yubikey
```

Lists all Yubikeys on this system

## PARAMETERS

### -OnlyOne
Return only one Yubikey

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

### -Serialnumber
Return only yubikey with serialnumber

```yaml
Type: Int32
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
