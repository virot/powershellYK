---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Rename-YubikeyOATHAccount

## SYNOPSIS

Rename OATH account

## SYNTAX

### Default (Default)

```
Rename-YubikeyOATHAccount -Account <Credential> [-NewAccountName <String>] [-NewIssuer <String>]
 [<CommonParameters>]
```

### __AllParameterSets

```
Rename-YubiKeyOATHAccount -Account <Credential> [-NewAccountName <string>] [-NewIssuer <string>]
 [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Rename OATH account

## EXAMPLES

### Example 1

```powershell
PS C:\> $Accounttochange = Get-YubikeyOATHAccount | Where-Object {$_.Issuer -eq 'Yubico Demo'}
PS C:\> Rename-YubikeyOATHAccount -Credential $Accounttochange -NewIssuer "powershellYK Demo"
```

Selects and updates the Issuer from 'Yubico Demo' to 'powershellYK Demo'

## PARAMETERS

### -Account

Account to rename

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
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -NewAccountName

New Account name

```yaml
Type: System.String
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

### -NewIssuer

New Issuer

```yaml
Type: System.String
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

