---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Connect-Yubikey

## SYNOPSIS

Connect the module to the YubiKey.

## SYNTAX

### Connect single Yubikey (Default)

```
Connect-YubiKey [<CommonParameters>]
```

### Connect provided Yubikey

```
Connect-YubiKey [[-YubiKey] <YubiKeyDevice>] [<CommonParameters>]
```

### Connect Yubikey with Serialnumber

```
Connect-YubiKey [-Serialnumber <int>] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

The `Connect-Yubikey` cmdlet allows the module connect to a YubiKey. The command allows specific YubiKey to be connected.

## EXAMPLES

### Example 1

```powershell
PS C:\> Connect-Yubikey
```

Try to connect to a single YubiKey, will fail if number of connected YubiKeys aren't one.

### Example 2

```powershell
PS C:\> Connect-Yubikey -Serialnumber -Serialnumber 12345
```

Connect to a specific YubiKey with serial 12345

## PARAMETERS

### -Serialnumber

Connect to YubiKey with Serialnumber

```yaml
Type: System.Nullable`1[System.Int32]
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Connect Yubikey with Serialnumber
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -YubiKey

Which YubiKey to connect to

```yaml
Type: Yubico.YubiKey.YubiKeyDevice
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Connect provided Yubikey
  Position: 0
  IsRequired: false
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

### Yubico.YubiKey.YubiKeyDevice

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS

