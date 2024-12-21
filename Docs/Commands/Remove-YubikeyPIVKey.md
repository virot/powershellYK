---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Remove-YubikeyPIVKey

## SYNOPSIS
Remove a key from a YubiKey PIV slot.

## SYNTAX

```
Remove-YubikeyPIVKey -Slot <PIVSlot> [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
This command will remove the key from the specified slot on the Yubikey. This will remove the key from the slot and the key will no longer be usable.

## EXAMPLES

### Example 1
```powershell
PS C:\> Remove-YubikeyPIVKey -Slot "PIV Authentication"
```

This command will remove the key from the PIV Authentication (0x9a) slot on the Yubikey.

## PARAMETERS

### -Slot
What slot to move a key from

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

### -Confirm
Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WhatIf
Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

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
