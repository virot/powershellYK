﻿---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Set-YubikeyOTP

## SYNOPSIS
Configure OTP slots

## SYNTAX

### Yubico OTP
```
Set-YubikeyOTP -Slot <PSObject> [-YubicoOTP] [-PublicID <Byte[]>] [-PrivateID <Byte[]>] [-SecretKey <Byte[]>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Static Password
```
Set-YubikeyOTP -Slot <PSObject> [-StaticPassword] -Password <SecureString> [-KeyboardLayout <KeyboardLayout>]
 [-AppendCarriageReturn] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Static Generated Password
```
Set-YubikeyOTP -Slot <PSObject> [-StaticPassword] -PasswordLength <Int32> [-KeyboardLayout <KeyboardLayout>]
 [-AppendCarriageReturn] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Allows the configuration of the Yubikey OTP slots. The Yubikey OTP slots can be configured with a static password, a HOTP code, Challange Response or a Yubico OTP configuration.

## EXAMPLES

### Example 1
```powershell
PS C:\> Set-YubikeyOTP -Slot 1 -StaticPassword -PasswordLength 16 -AppendCarriageReturn
```

Created a static password with a length of 16 characters and appends a carriage return.

### Example 2
```powershell
PS C:\> Set-YubikeyOTP -Slot 1 -StaticPassword -KeyboardLayout sv_SE -Password (Read-Host -AsSecureString "Password")
Password: ******
```

Creates a statick password for a Swedish keyboard layout.

## PARAMETERS

### -AppendCarriageReturn
Append carriage return

```yaml
Type: SwitchParameter
Parameter Sets: Static Password, Static Generated Password
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -KeyboardLayout
Keyboard to be used

```yaml
Type: KeyboardLayout
Parameter Sets: Static Password, Static Generated Password
Aliases:
Accepted values: ModHex, en_US, en_UK, de_DE, fr_FR, it_IT, es_US, sv_SE

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Password
Static password that will be set

```yaml
Type: SecureString
Parameter Sets: Static Password
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PasswordLength
Static password that will be set

```yaml
Type: Int32
Parameter Sets: Static Generated Password
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PrivateID
Sets the Private ID, defaults to random 6 bytes

```yaml
Type: Byte[]
Parameter Sets: Yubico OTP
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PublicID
Sets the Public ID, defaults to the serialnumber

```yaml
Type: Byte[]
Parameter Sets: Yubico OTP
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SecretKey
Sets the Secret key, defaults to random 16 bytes

```yaml
Type: Byte[]
Parameter Sets: Yubico OTP
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
Type: PSObject
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -StaticPassword
Allows configuration with all defaults

```yaml
Type: SwitchParameter
Parameter Sets: Static Password, Static Generated Password
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -YubicoOTP
Allows configuration with all defaults

```yaml
Type: SwitchParameter
Parameter Sets: Yubico OTP
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