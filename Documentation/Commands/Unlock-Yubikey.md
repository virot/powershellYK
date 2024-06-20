---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Unlock-Yubikey

## SYNOPSIS
Unlocks the configuration lock on the Yubikey

## SYNTAX

```
Unlock-Yubikey -LockCode <Byte[]> [<CommonParameters>]
```

## DESCRIPTION
Allow the Yubikey to be configured once more.

## EXAMPLES

### Example 1
```powershell
PS C:\> $Lockcode = [byte[]](1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)
PS C:\> Unlock-Yubikey -LockCode $Lockcode
WARNING: Please remove and reinsert Yubikey
```

Removes the configuration lock on the Yubikey

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
