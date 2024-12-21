---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Move-YubikeyPIV

## SYNOPSIS
Move a key from one slot to another

## SYNTAX

```
Move-YubikeyPIV -SourceSlot <PIVSlot> -DestinationSlot <PIVSlot> [-MigrateCertificate] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## DESCRIPTION
This command will move a key from one slot to another. This is useful if you want to change the slot a key is in.

## EXAMPLES

### Example 1
```powershell
PS C:\> Move-YubikeyPIVKey -SourceSlot "Digital Signature" -DestinationSlot "Card Authentication"
```

That command would move the key in the Digital Signature slot to the Card Authentication slot.

## PARAMETERS

### -DestinationSlot
What slot to move a key to

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

### -MigrateCertificate
Move the certificate along

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

### -SourceSlot
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
