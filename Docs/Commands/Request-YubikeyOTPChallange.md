---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Request-YubikeyOTPChallange

## SYNOPSIS

Send Challaenge to YubiKey.

## SYNTAX

### Default (Default)

```
Request-YubikeyOTPChallange -Slot <Slot> -Phrase <PSObject> [-YubikeyOTP <Boolean>]
 [<CommonParameters>]
```

### __AllParameterSets

```
Request-YubiKeyOTPChallange -Slot <Slot> -Phrase <psobject> [-YubikeyOTP <bool>]
 [<CommonParameters>]
```

## ALIASES

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
Type: System.Management.Automation.PSObject
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
AcceptedValues: []
HelpMessage: ''
```

### -Slot

YubiOTP Slot

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

### -YubikeyOTP

Use YubiOTP over HMAC-SHA1

```yaml
Type: System.Boolean
DefaultValue: None
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

