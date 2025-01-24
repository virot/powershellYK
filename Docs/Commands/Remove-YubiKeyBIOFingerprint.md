---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Remove-YubiKeyBIOFingerprint

## SYNOPSIS
Removes a selected fingerprint template from the YubiKey Bio or YubiKey Bio Multi-Protocol Edition (MPE).

## SYNTAX

### Remove using Name
```
Remove-YubiKeyBIOFingerprint -Name <String> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Remove using ID
```
Remove-YubiKeyBIOFingerprint -ID <String> [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
{{ Fill in the Description }}

## EXAMPLES

### Example 1
```powershell
PS C:\> Remove-YubikeyBIOFingerprint -Name "left index"
[Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help (default is "Y"): Y
Fingerprint 'left index' successfully deleted.
```

A fingerprint template is removed by name.

### Example 2
```powershell
Remove-YubikeyBIOFingerprint -ID 23FC
[Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help (default is "Y"): Y
Fingerprint '23FC' successfully deleted.
```

A fingerprint template is removed by ID.

## PARAMETERS

### -ID
ID of finger to remove

```yaml
Type: String
Parameter Sets: Remove using ID
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
Name of finger to remove

```yaml
Type: String
Parameter Sets: Remove using Name
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
