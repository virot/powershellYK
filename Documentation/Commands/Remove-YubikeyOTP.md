---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Remove-YubikeyOTP

## SYNOPSIS
Remove Yubikey OTP slot

## SYNTAX

```
Remove-YubikeyOTP -Slot <PSObject> [<CommonParameters>]
```

## DESCRIPTION
Remove the OTP configuration from a slot on the Yubikey

## EXAMPLES

### Example 1
```powershell
PS C:\> Remove-YubikeyOTP -Slot 1
```

Removes the OTP configuration from slot 1 (Short press)

### Example 2
```powershell
PS C:\> $Slot = [Yubico.YubiKey.Otp.Slot]::ShortPress
PS C:\> Remove-YubikeyOTP -Slot $Slot
```

Removes the OTP configuration from slot 1 (Short press)

## PARAMETERS

### -Slot
Yubikey OTP Slot

```yaml
Type: PSObject
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
