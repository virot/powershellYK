---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# New-YubiKeyPIVKey

## SYNOPSIS
Create a new private key

## SYNTAX

```
New-YubiKeyPIVKey [-Slot] <PIVSlot> [-PinPolicy <PivPinPolicy>] [-TouchPolicy <PivTouchPolicy>] [-PassThru]
 [-WhatIf] [-Confirm] -Algorithm <PivAlgorithm> [<CommonParameters>]
```

## DESCRIPTION
This cmdlet will create a new key, this can be done with either RSA or ECC keys.

## EXAMPLES

### Example 1
```powershell
PS C:\> New-YubikeyPIVKey -Slot 0x9a -Algorithm EccP384
```

Creates a new Elliptic curve P-384 key in slot 0x9a.

### Example 2
```powershell
PS C:\> New-YubikeyPIVKey -Slot 0x9a -Algorithm RSA2048 -PinPolicy Never
```

Create a RSA2048 in slot 0x9a with a PIN policy of never.

### Example 3
```powershell
PS C:\> New-YubikeyPIVKey -Slot 0x9a -Algorithm EccP384 -TouchPolicy Cached
```

Create a RSA2048 in slot 0x9a with a touch policy of cached

## PARAMETERS

### -Algorithm
What algorithm to use

```yaml
Type: PivAlgorithm
Parameter Sets: (All)
Aliases:
Accepted values: Rsa1024, Rsa2048, Rsa3072, Rsa4096, EccP256, EccP384

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru
Returns an object that represents the item with which you're working. By default, this cmdlet doesn't generate any output.

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

### -PinPolicy
PinPolicy

```yaml
Type: PivPinPolicy
Parameter Sets: (All)
Aliases:
Accepted values: None, Never, Once, Always, MatchOnce, MatchAlways, Default

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Slot
What slot to create a new key for

```yaml
Type: PIVSlot
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -TouchPolicy
TouchPolicy

```yaml
Type: PivTouchPolicy
Parameter Sets: (All)
Aliases:
Accepted values: Default, Never, Always, Cached

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
