---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Request-YubikeyOATHCode

## SYNOPSIS

Displays TOTP / HOTP codes for YubiKey OATH credentials.

## SYNTAX

### All (Default)

```
Request-YubiKeyOATHCode -All [<CommonParameters>]
```

### Specific

```
Request-YubiKeyOATHCode -Account <Credential> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Displays TOTP / HOTP codes for Yubikey OATH credentials

## EXAMPLES

### Example 1

```powershell
PS C:\> Request-YubikeyOATHCode

                                 Value  validFrom           validUntil
              Name

-------------------------------- -----  ---------           ----------
Issuer Issuer:dsa                221624 2024-06-15 19:26:00 2024-06-15 19:26:30
```

List the current code for all OATH credentials

## PARAMETERS

### -Account

Account to generate code for

```yaml
Type: Yubico.YubiKey.Oath.Credential
DefaultValue: None
SupportsWildcards: false
Aliases:
- Credential
ParameterSets:
- Name: Specific
  Position: Named
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -All

Get codes for all accounts

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: All
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

### Yubico.YubiKey.Oath.Credential

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

