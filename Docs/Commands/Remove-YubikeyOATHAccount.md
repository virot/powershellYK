---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Remove-YubikeyOATHAccount

## SYNOPSIS

Removes an account from the YubiKey OATH application.

## SYNTAX

### Default (Default)

```
Remove-YubikeyOATHAccount -Account <Credential> [<CommonParameters>]
```

### __AllParameterSets

```
Remove-YubiKeyOATHAccount -Account <Credential> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Removes an account from the Yubikey OATH application

## EXAMPLES

### Example 1

```powershell
PS C:\> Remove-YubikeyOATHAccount -Account (Get-YubikeyOATHAccount | ?{$_.Issuer -eq 'Yubico Demo'})
```

Removes the account with the issuer 'Yubico Demo'.

## PARAMETERS

### -Account

Credential to remove

```yaml
Type: Yubico.YubiKey.Oath.Credential
DefaultValue: None
SupportsWildcards: false
Aliases:
- Credential
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: true
  ValueFromPipeline: true
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

### Yubico.YubiKey.Oath.Credential

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

