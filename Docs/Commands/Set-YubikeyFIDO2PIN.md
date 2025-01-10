---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Set-YubiKeyFIDO2PIN

## SYNOPSIS
Set the PIN for the FIDO2 application on the YubiKey.

## SYNTAX

```
Set-YubiKeyFIDO2PIN [-OldPIN <SecureString>] -NewPIN <SecureString> [<CommonParameters>]
```

## DESCRIPTION
Set the PIN for the FIDO2 application on the YubiKey.

## EXAMPLES

### Example 1
```powershell
PS C:\> Set-YubikeyFIDO2PIN -NewPIN (ConvertTo-SecureString -String "123456" -Force -AsPlainText)
```

Sets the initial PIN or update a connected YubiKeys FIDO2 application PIN.

### Example 2
```powershell
PS C:\> Set-YubikeyFIDO2PIN -OldPIN (ConvertTo-SecureString -String "123456" -Force -AsPlainText) -NewPIN (ConvertTo-SecureString -String "234567" -Force -AsPlainText)
```

Update the FIDO2 application PIN on an unconnected YubiKey.

## PARAMETERS

### -NewPIN
New PIN code to set for the FIDO2 module.

```yaml
Type: SecureString
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OldPIN
Old PIN, required to change the PIN code.

```yaml
Type: SecureString
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
