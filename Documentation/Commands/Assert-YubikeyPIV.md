---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Assert-YubikeyPIV

## SYNOPSIS
Create attestation certificate

## SYNTAX

### ExportToFile
```
Assert-YubikeyPIV -Slot <Byte> -OutFile <String> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### DisplayOnScreen
```
Assert-YubikeyPIV -Slot <Byte> [-PEMEncoded] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Create and export attestation certificate for a slot

## EXAMPLES

### Example 1
```powershell
PS C:\> Assert-YubikeyPIV -Slot 0x9a -OutFile attestation.cer
```

Creates and exports the attestation certificate for slot 0x9a

## PARAMETERS

### -OutFile
Location of attestation certificate

```yaml
Type: String
Parameter Sets: ExportToFile
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PEMEncoded
Encode output as PEM

```yaml
Type: SwitchParameter
Parameter Sets: DisplayOnScreen
Aliases:

Required: False
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

### -Slot
Yubikey PIV Slot

```yaml
Type: Byte
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
