---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Set-YubiKeyOTPSlotAccessCode

## SYNOPSIS
Sets, changes or removes the OTP slot access code for a YubiKey.
he access code protects OTP slot configurations from unauthorized modifications.

## SYNTAX

### SetNewAccessCode
```
Set-YubiKeyOTPSlotAccessCode -Slot <Slot> [-AccessCode <String>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### ChangeAccessCode
```
Set-YubiKeyOTPSlotAccessCode -Slot <Slot> -AccessCode <String> -CurrentAccessCode <String> [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### RemoveAccessCode
```
Set-YubiKeyOTPSlotAccessCode -Slot <Slot> -CurrentAccessCode <String> [-RemoveAccessCode] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## DESCRIPTION
Sets, changes or removes the OTP slot access code for a YubiKey.
The access code protects OTP slot configurations from unauthorized modifications.
Access codes are 6 bytes in length, provided as 12-character hex strings.

## EXAMPLES

### Example 1
```powershell
PS C:\> Set-YubiKeySlotAccessCode -Slot LongPress -AccessCode "010203040506"
```

Set a new access code for a slot (when no access code exists)

### Example 2
```powershell
PS C:\> Set-YubiKeyOTPSlotAccessCode -Slot ShortPress -CurrentAccessCode "010203040506" -AccessCode "060504030201"
```

Change an existing slot access code

### Example 3
```powershell
PS C:\> Set-YubiKeyOTPSlotAccessCode -Slot LongPress -CurrentAccessCode "010203040506" -RemoveAccessCode
```

Remove slot access code protection (set to all zeros)

## PARAMETERS

### -AccessCode
New access code (12-character hex string)

```yaml
Type: String
Parameter Sets: SetNewAccessCode
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

```yaml
Type: String
Parameter Sets: ChangeAccessCode
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CurrentAccessCode
Current access code (12-character hex string)

```yaml
Type: String
Parameter Sets: ChangeAccessCode, RemoveAccessCode
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -RemoveAccessCode
Remove access code protection

```yaml
Type: SwitchParameter
Parameter Sets: RemoveAccessCode
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Slot
Yubikey OTP Slot

```yaml
Type: Slot
Parameter Sets: (All)
Aliases:
Accepted values: None, ShortPress, LongPress

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
