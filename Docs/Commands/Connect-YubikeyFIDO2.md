---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Connect-YubiKeyFIDO2

## SYNOPSIS

Connect to the FIDO2 session.

## SYNTAX

### Default (Default)

```
Connect-YubiKeyFIDO2 -PIN <SecureString> [<CommonParameters>]
```

### __AllParameterSets

```
Connect-YubiKeyFIDO2 -PIN <securestring> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Allows FIDO2 commands to be sent to the YubiKey
Must be run as Administrator

## EXAMPLES

### Example 1

```powershell
PS C:\> Connect-YubikeyFIDO2

cmdlet Connect-YubikeyFIDO2 at command pipeline position 1
Supply values for the following parameters:
(Type !? for Help.)
PIN: ******
```

Connect to FIDO2 module

### Example 2

```powershell
PS C:\> $PIN = Read-Host -AsSecureString 'PIN'
PIN: ******
PS C:\> Connect-YubikeyFIDO2 -PIN $PIN
```

Connect to FIDO2 module with and a stored pin requested from the commandline

### Example 3

```powershell
PS C:\> $PIN = ConvertTo-SecureString -String "123456" -AsPlainText -Force
PS C:\> Connect-YubikeyFIDO2 -PIN $PIN
```

Connect to FIDO2 module with a stored pin requested constructed from code

## PARAMETERS

### -PIN

PIN

```yaml
Type: System.Security.SecureString
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

