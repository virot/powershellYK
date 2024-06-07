﻿---
external help file: VirotYubikey.dll-Help.xml
Module Name: virotYubikey
online version:
schema: 2.0.0
---

# Connect-YubikeyPIV

## SYNOPSIS
Connect PIV module

## SYNTAX

```
Connect-YubikeyPIV [-ManagementKey <String>] [[-PIN] <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Connects the PIV module for the currently connected Yubikey, with PIN and Managementkey as needed

## EXAMPLES

### Example 1
```powershell
PS C:\> Connect-YubikeyPIV
```

Connect to PIV module with default PIN and ManagmentKey

### Example 2
```powershell
PS C:\> Connect-YubikeyPIV -PIN (Read-Host -MaskInput 'PIN')
```

Connect to PIV module with default Managementkey and requests the PIN from the commandline

## PARAMETERS

### -ManagementKey
ManagementKey

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

### -PIN
PIN

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS