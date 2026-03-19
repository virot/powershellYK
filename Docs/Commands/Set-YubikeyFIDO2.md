---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Set-YubiKeyFIDO2

## SYNOPSIS

Allows settings FIDO2 options.

## SYNTAX

### Set PIN minimum length

```
Set-YubiKeyFIDO2 -MinimumPINLength <int> [<CommonParameters>]
```

### Set force PIN change

```
Set-YubiKeyFIDO2 -ForcePINChange [<CommonParameters>]
```

### Send MinimumPIN to RelyingParty

```
Set-YubiKeyFIDO2 -MinimumPINRelyingParty <String> [<CommonParameters>]
```

### Set PIN

```
Set-YubiKeyFIDO2 -NewPIN <securestring> [-SetPIN] [-OldPIN <securestring>] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Allows the setting of PIN code and minimum PIN length.

## EXAMPLES

### Example 1

```powershell
PS C:\> Set-YubikeyFIDO2 -SetPIN

cmdlet Set-YubikeyFIDO2 at command pipeline position 1
Supply values for the following parameters:
(Type !? for Help.)
NewPIN: ******
```

Set a PIN on the Yubikey

### Example 2

```powershell
PS C:\> Connect-YubikeyFIDO2

cmdlet Connect-YubikeyFIDO2 at command pipeline position 1
Supply values for the following parameters:
(Type !? for Help.)
PIN: ******
PS C:\> Set-YubikeyFIDO2 -SetPIN

cmdlet Set-YubikeyFIDO2 at command pipeline position 1
Supply values for the following parameters:
(Type !? for Help.)
NewPIN: ******
```

**NOTE**: The setting requires PIN be provided first using the `Connect-YubikeyFIDO2` command.

### Example

```powershell
PS C:\> Set-YubikeyFIDO2 -ForcePINChange
```

**NOTE**: The setting requires PIN be provided first using the `Connect-YubikeyFIDO2` command.

### MinimumPINLength

Set the _minimum_ length of the PIN as supported by YubiKeys with firmware `5.7` or later.
When set, any PIN selected by the user must equal to or longer than the enforced value.

```yaml
Type: Int32
Parameter Sets: Set PIN minimum length
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### MinimumPINRelyingParty

To which RelyingParty should minimum PIN be sent

```yaml
Type: String
Parameter Sets: Send MinimumPIN to RelyingParty
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### NewPIN

New PIN

```yaml
Type: SecureString
Parameter Sets: Set PIN
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### OldPIN

Old PIN, required to change the PIN code.

```yaml
Type: SecureString
Parameter Sets: Set PIN
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### SetPIN

Easy access to Set new PIN

```yaml
Type: SwitchParameter
Parameter Sets: Set PIN
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## PARAMETERS

### -ForcePINChange

Enable the **_forceChangePin__** flag as supported by YubiKeys with firmware `5.7` or later.
When set, the feature will force the user to change the FIDO2 applet PIN on first use.


```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Set force PIN change
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -MinimumPINLength

Set the minimum length of the PIN

```yaml
Type: System.Nullable`1[System.Int32]
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Set PIN minimum length
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -MinimumPINRelyingParty

To which RelyingParty should minimum PIN be sent

```yaml
Type: System.String
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Send MinimumPIN to RelyingParty
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -NewPIN

New PIN code to set for the FIDO2 module.

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
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -SetPIN

Easy access to Set new PIN

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Set PIN
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

