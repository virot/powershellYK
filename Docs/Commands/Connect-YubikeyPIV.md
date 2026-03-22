---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Connect-YubikeyPIV

## SYNOPSIS

Connect PIV module

## SYNTAX

### PIN (Default)

```
Connect-YubiKeyPIV -PIN <securestring> [<CommonParameters>]
```

### PIN&Management

```
Connect-YubiKeyPIV -ManagementKey <psobject> -PIN <securestring> [<CommonParameters>]
```

### Management

```
Connect-YubiKeyPIV -ManagementKey <psobject> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Connects the PIV module for the currently connected YubiKey, with PIN and Management Key as needed

## EXAMPLES

### Example 1

```powershell
PS C:\> Connect-YubikeyPIV

cmdlet Connect-YubikeyPIV at command pipeline position 1
Supply values for the following parameters:
(Type !? for Help.)
PIN: ******
```

Connect to PIV module with default Managment Key

### Example 2

```powershell
PS C:\> $PIN = Read-Host -AsSecureString 'PIN'
PIN: ******
PS C:\> Connect-YubikeyPIV -PIN $PIN
```

Connect to PIV module with default Management key and a stored pin requested from the command line

### Example 3

```powershell
PS C:\> $PIN = ConvertTo-SecureString -String "123456" -AsPlainText -Force
PS C:\> Connect-YubikeyPIV -PIN $PIN
```

Connect to PIV module with default Managementkey and a stored PIN requested constructed from code

## PARAMETERS

### -ManagementKey

Management key

```yaml
Type: System.Management.Automation.PSObject
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: PIN&Management
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: Management
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PIN

PIN

```yaml
Type: System.Security.SecureString
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: PIN&Management
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: PIN
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

