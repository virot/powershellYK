---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Reset-YubiKeyFIDO2

## SYNOPSIS
Reset a YubiKey FIDO2 device to factory settings.

## SYNTAX

```
Reset-YubiKeyFIDO2 [-Interactive <Boolean>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Resets the YubiKey FIDO2 applet to factory settings. This will remove all stored credentials and reset the applet to factory settings.
This REQUIRES the YubiKey to be (re)inserted at a maximum `5` seconds _before_ running the command.

## EXAMPLES

### Example 1
```powershell
PS C:\> Reset-YubikeyFIDO2
```

Resets the YubiKey FIDO2 applet to factory settings.

## PARAMETERS

### -Interactive
Interactive mode for removing and reinserting YubiKey

```yaml
Type: Boolean
Parameter Sets: (All)
Aliases:

Required: False
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
Shows what would happen if the cmdlet runs. The cmdlet is not run.

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
