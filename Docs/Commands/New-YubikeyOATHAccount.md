---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# New-YubikeyOATHAccount

## SYNOPSIS

Created a TOTP or HOTP account

## SYNTAX

### TOTP (Default)

```
New-YubiKeyOATHAccount -TOTP -Issuer <string> -Accountname <string> -Secret <string>
 -Period <CredentialPeriod> [-Algorithm <HashAlgorithm>] [-Digits <int>] [<CommonParameters>]
```

### HOTP

```
New-YubiKeyOATHAccount -HOTP -Issuer <string> -Accountname <string> -Secret <string>
 [-Algorithm <HashAlgorithm>] [-Digits <int>] [-Counter] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Creates new account that can be viewed in the Yubikey Authenticator or using Request-YubikeyOATHCode.

## EXAMPLES

### Example 1

```powershell
PS C:\> New-YubikeyOATHAccount -TOTP -Accountname "powershellYK" -Issuer "Demo" -Period 60 -Secret (Read-Host -Prompt 'Secret' -MaskInput)
Secret: *****************
```

Creates en entry

## PARAMETERS

### -Accountname

Accountname
Account name
Account name
Account name

```yaml
Type: System.String
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

### -Algorithm

Algorithm

```yaml
Type: Yubico.YubiKey.Oath.HashAlgorithm
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
AcceptedValues:
- SHA1
- SHA256
- SHA512
HelpMessage: ''
```

### -Counter

Counter

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

### -Digits

Digits

```yaml
Type: System.Int32
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

### -HOTP

Type of OATH

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: HOTP
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Issuer

Issuer

```yaml
Type: System.String
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

### -Period

Period for credential

```yaml
Type: Yubico.YubiKey.Oath.CredentialPeriod
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: TOTP
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- Undefined
- Period15
- Period30
- Period60
HelpMessage: ''
```

### -Secret

Secret

```yaml
Type: System.String
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

### -TOTP

Type of OATH

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: TOTP
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

