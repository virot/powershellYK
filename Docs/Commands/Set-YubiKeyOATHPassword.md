---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Set-YubiKeyOATHPassword

## SYNOPSIS

Set the password for the YubiKey OATH application.

## SYNTAX

### Default (Default)

```
Set-YubiKeyOATHPassword -OldPassword <SecureString> -NewPassword <SecureString> [<CommonParameters>]
```

### Password

```
Set-YubiKeyOATHPassword -OldPassword <securestring> -NewPassword <securestring> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Set the password for the YubiKey OATH application.

## EXAMPLES

### Example 1

```powershell
PS C:\>
```

{{ Add example description here }}

## PARAMETERS

### -NewPassword

New password provided as a SecureString.

```yaml
Type: System.Security.SecureString
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Password
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -OldPassword

Current password provided as a SecureString.

```yaml
Type: System.Security.SecureString
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Password
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

