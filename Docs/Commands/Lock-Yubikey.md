---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Lock-Yubikey

## SYNOPSIS
Lock the YubiKey configuration

## SYNTAX

```
Lock-Yubikey -LockCode <Byte[]> [<CommonParameters>]
```

## DESCRIPTION
Locks the YubiKey behind a 16 byte lock code. This lock code is required to unlock the Yubikey configuration.

## EXAMPLES

### Example 1
```powershell
PS C:\> $Lockcode = [byte[]](1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)
PS C:\> Lock-Yubikey -LockCode $Lockcode
WARNING: Please remove and reinsert Yubikey
```

Locks the configuration so it requires a hex code of 0102030405060708090A0B0C0D0E0F10 to unlock.

## PARAMETERS

### -LockCode
LockCode for Yubikey

```yaml
Type: Byte[]
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
