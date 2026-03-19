---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Set-YubiKeyFIDO2PIN

## SYNOPSIS

Set the PIN for the FIDO2 application on the YubiKey.

## SYNTAX

### Default (Default)

```
Set-YubiKeyFIDO2PIN -NewPIN <SecureString> [-OldPIN <SecureString>] [<CommonParameters>]
```

### Set PIN

```
Set-YubiKeyFIDO2PIN -OldPIN <securestring> -NewPIN <securestring> [<CommonParameters>]
```

## ALIASES

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

New PIN code to set for the FIDO applet.

```yaml
Type: System.Security.SecureString
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Set PIN
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -OldPIN

Old PIN, required to change the PIN code.

```yaml
Type: System.Security.SecureString
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Set PIN
  Position: Named
  IsRequired: true
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

