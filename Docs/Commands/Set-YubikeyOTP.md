---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Set-YubiKeyOTP

## SYNOPSIS

Configure OTP slots

## SYNTAX

### Yubico OTP

```
Set-YubiKeyOTP -Slot <Slot> [-YubicoOTP] [-PublicID <byte[]>] [-PrivateID <byte[]>]
 [-SecretKey <byte[]>] [-Upload] [-AccessCode <string>] [-CurrentAccessCode <string>] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

### Static Password

```
Set-YubiKeyOTP -Slot <Slot> -Password <securestring> [-StaticPassword]
 [-KeyboardLayout <KeyboardLayout>] [-AppendCarriageReturn] [-AccessCode <string>]
 [-CurrentAccessCode <string>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Static Generated Password

```
Set-YubiKeyOTP -Slot <Slot> -PasswordLength <int> [-StaticGeneratedPassword]
 [-KeyboardLayout <KeyboardLayout>] [-AppendCarriageReturn] [-AccessCode <string>]
 [-CurrentAccessCode <string>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### ChallengeResponse

```
Set-YubiKeyOTP -Slot <Slot> [-ChallengeResponse] [-SecretKey <byte[]>]
 [-Algorithm <ChallengeResponseAlgorithm>] [-RequireTouch] [-AccessCode <string>]
 [-CurrentAccessCode <string>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### HOTP

```
Set-YubiKeyOTP -Slot <Slot> [-SecretKey <byte[]>] [-AppendCarriageReturn] [-SendTabFirst] [-HOTP]
 [-Base32Secret <string>] [-HexSecret <string>] [-Use8Digits] [-AccessCode <string>]
 [-CurrentAccessCode <string>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

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

### -AccessCode

New access code (12-character hex string)

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Algorithm

Algorithm for Challenge-Response

```yaml
Type: Yubico.YubiKey.Otp.ChallengeResponseAlgorithm
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChallengeResponse
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- None
- YubicoOtp
- HmacSha1
HelpMessage: ''
```

### -AppendCarriageReturn

Append carriage return (Enter). This parameter can improve user experience
and login performance by effectively submitting the credential on the input
field and "pressing Enter" on behalf of the user.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Static Password
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: Static Generated Password
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: HOTP
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Base32Secret

Base32 encoded secret key for HOTP

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: HOTP
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ChallengeResponse

Allows for Challenge-Response configuration with all defaults

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChallengeResponse
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Confirm

Prompts you for confirmation before running the cmdlet.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases:
- cf
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CurrentAccessCode

Current access code (12-character hex string)

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -HexSecret

Hex encoded secret key for HOTP

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: HOTP
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -HOTP

Allows configuration of HOTP mode

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: HOTP
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -KeyboardLayout

Keyboard layout to be used

```yaml
Type: Yubico.Core.Devices.Hid.KeyboardLayout
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Static Password
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: Static Generated Password
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- ModHex
- en_US
- en_UK
- de_DE
- fr_FR
- it_IT
- es_US
- sv_SE
HelpMessage: ''
```

### -Password

Static password that will be set

```yaml
Type: System.Security.SecureString
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Static Password
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PasswordLength

Length of static password that will be set.

```yaml
Type: System.Int32
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Static Generated Password
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PrivateID

Sets the Private ID, defaults to random 6 bytes

```yaml
Type: System.Byte[]
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Yubico OTP
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PublicID

Sets the Public ID, defaults to YubiKey serial number

```yaml
Type: System.Byte[]
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Yubico OTP
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RequireTouch

Require Touch

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ChallengeResponse
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -SecretKey

Sets the Secret Key, defaults to random 16 bytes

```yaml
Type: System.Byte[]
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Yubico OTP
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: ChallengeResponse
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: HOTP
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -SendTabFirst

Send TAB before passcode to help navigate UI

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: HOTP
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Slot

Yubikey OTP Slot

```yaml
Type: Yubico.YubiKey.Otp.Slot
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- None
- ShortPress
- LongPress
HelpMessage: ''
```

### -StaticGeneratedPassword

Allows configuration with all defaults

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Static Generated Password
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -StaticPassword

Allows configuration with all defaults

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Static Password
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Upload

Upload to YubiCloud

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Yubico OTP
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Use8Digits

Use 8 digits instead of 6 for HOTP

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: HOTP
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -WhatIf

Runs the command in a mode that only reports what would happen without performing the actions.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases:
- wi
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -YubicoOTP

Allows configuration with all defaults

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Yubico OTP
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

