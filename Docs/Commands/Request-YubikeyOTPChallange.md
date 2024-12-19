---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Request-YubikeyOTPChallange

## SYNOPSIS
Send Challaenge to YubiKey.

## SYNTAX

```
Request-YubikeyOTPChallange -Slot <PSObject> -Phrase <PSObject> [-YubikeyOTP <Boolean>] [<CommonParameters>]
```

## DESCRIPTION
Allow the sending of a yubikey challenge to the yubikey device.

## EXAMPLES

### Example 1
```powershell
PS C:\> Request-YubikeyOTPChallange -Slot ShortPress -Phrase "01"
08D0DDD5DA2CC01566947555AA49F400F4F6F8A4
```

Sending the challenge phrase "01" to the yubikey device in the ShortPress slot.

### Example 2
```powershell
PS C:\> $Challaenge = [byte[]](01)
PS C:\> Request-YubikeyOTPChallange -Slot ShortPress -Phrase $Challaenge
08D0DDD5DA2CC01566947555AA49F400F4F6F8A4
```

Sending the challenge phrase "01" to the yubikey device in the ShortPress slot.

## PARAMETERS

### -Phrase
Phrase

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

### -YubikeyOTP
Use YubicoOTP over HMAC-SHA1

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
