---
external help file: VirotYubikey.dll-Help.xml
Module Name: virotYubikey
online version:
schema: 2.0.0
---

# New-YubikeyPIVSelfSign

## SYNOPSIS
Create a self signed certificate

## SYNTAX

```
New-YubikeyPIVSelfSign -Slot <Byte> [-Subjectname <String>] [-HashAlgorithm <HashAlgorithmName>]
 [-ProgressAction <ActionPreference>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
This cmdlet creates a selfsigned certificate for a private key.

## EXAMPLES

### Example 1
```powershell
PS C:\> New-YubikeyPIVSelfSign -Slot 0x9a
```

Creates a selfsigned certificate and installs into the 0x9a slot.

## PARAMETERS

### -HashAlgorithm
HashAlgoritm

```yaml
Type: HashAlgorithmName
Parameter Sets: (All)
Aliases:
Accepted values: SHA1, SHA256, SHA384, SHA512

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
Sign a self signed cert for slot

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

### -Subjectname
Subjectname of certificate

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
