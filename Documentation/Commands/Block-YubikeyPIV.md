---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Block-YubikeyPIV

## SYNOPSIS
Block out PIN or PUK codes

## SYNTAX

### BlockBoth
```
Block-YubikeyPIV [-PIN] [-PUK] [-ProgressAction <ActionPreference>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### BlockPIN
```
Block-YubikeyPIV [-PIN] [-ProgressAction <ActionPreference>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### BlockPUK
```
Block-YubikeyPIV [-PUK] [-ProgressAction <ActionPreference>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Allows you to block the PIN and/or PUK for Yubikey PIV

## EXAMPLES

### Example 1
```powershell
PS C:\> Block-YubikeyPIV -PUK
```

Block the PUK code.

### Example 2
```powershell
PS C:\> Block-YubikeyPIV -PUK -PIN
```

This blocks both the PIN and the PUK.

## PARAMETERS

### -PIN
Blocks the PIN

```yaml
Type: SwitchParameter
Parameter Sets: BlockBoth, BlockPIN
Aliases:

Required: True
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

### -PUK
Block the PUK

```yaml
Type: SwitchParameter
Parameter Sets: BlockBoth, BlockPUK
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
