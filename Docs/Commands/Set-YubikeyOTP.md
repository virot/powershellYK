---
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
 [-Upload] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Static Password
```
Set-YubikeyOTP -Slot <PSObject> [-StaticPassword] -Password <SecureString> [-KeyboardLayout <KeyboardLayout>]
 [-AppendCarriageReturn] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Static Generated Password
```
Set-YubikeyOTP -Slot <PSObject> [-StaticGeneratedPassword] -PasswordLength <Int32>
 [-KeyboardLayout <KeyboardLayout>] [-AppendCarriageReturn] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### ChallengeResponse
```
Set-YubikeyOTP -Slot <PSObject> [-ChallengeResponse] [-SecretKey <Byte[]>]
 [-Algorithm <ChallengeResponseAlgorithm>] [-RequireTouch] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Allows the configuration of the YubiKey OTP slots (2). The YubiKey OTP slots can be configured with: 

- Static password (or a split password), 
- HOTP (OATH) secret for One Time Passwords (OTPs), 
- Challange-Response (YubiOTP or HMAC-SHA1 format)
- YubiOTP (Yubico OTP)

## EXAMPLES

### Example 1
```powershell
PS C:\> Set-YubikeyOTP -Slot 1 -StaticPassword -PasswordLength 16 -AppendCarriageReturn
```

Creates a static password with a length of 16 characters and appends a carriage return (Enter).

### Example 2
```powershell
PS C:\> Set-YubikeyOTP -Slot 1 -StaticPassword -KeyboardLayout sv_SE -Password (Read-Host -AsSecureString "Password")
Password: ******
```

Creates a static password using the Swedish keyboard layout.

### Example 3
```powershell
PS C:\> Set-YubikeyOTP -Slot 1 -ChallengeResponse

SecretKeyByte         SecretKey
-------------         ---------
{164, 239, 163, 131…} A4EFA38352F551FF4E1711322BDD1D952CF659DF
```

Generates a new Challenge-Response Secret Key. The key will be printed after it is stored.

### Example 4
```powershell
PS C:\> $OldKey = [powershellYK.support.HexConverter]::StringToByteArray('A4EFA38352F551FF4E1711322BDD1D952CF659DF')
PS C:\> Set-YubikeyOTP -Slot 1 -ChallengeResponse -SecretKey $OldKey
```

Generate a new Challenge-Response Secret Key. The key will be printed after it is stored.

## PARAMETERS

### -Algorithm
Algorithm for Challange-Response.

```yaml
Type: ChallengeResponseAlgorithm
Parameter Sets: ChallengeResponse
Aliases:
Accepted values: None, YubicoOtp, HmacSha1

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AppendCarriageReturn
Append carriage return (Enter). This parameter can improve user experience
and login performance by effectively submitting the credential on the input
field and "pressing Enter" on behalf of the user.

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

### -ChallengeResponse
Allows for Challenge-Response configuration with all defaults.

```yaml
Type: SwitchParameter
Parameter Sets: ChallengeResponse
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -KeyboardLayout
Keyboard layout to be used.

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
Static password that will be set.

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
Length of static password that will be set.

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
Sets the Private ID, defaults to random 6 bytes.

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
Sets the Public ID, defaults to the YubiKey serial number.

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

### -RequireTouch
Require Touch.

```yaml
Type: SwitchParameter
Parameter Sets: ChallengeResponse
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SecretKey
Sets the Secret Key, defaults to random 16 bytes.

```yaml
Type: Byte[]
Parameter Sets: Yubico OTP, ChallengeResponse
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Slot
Yubikey OTP Slot.

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

### -StaticGeneratedPassword
Allows configuration with all defaults.

```yaml
Type: SwitchParameter
Parameter Sets: Static Generated Password
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -StaticPassword
Allows configuration with all defaults.

```yaml
Type: SwitchParameter
Parameter Sets: Static Password
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Upload
Upload to YubiCloud.

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

### -YubicoOTP
Allows configuration with all defaults.

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
