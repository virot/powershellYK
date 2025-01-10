---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Set-YubiKeyFIDO2

## SYNOPSIS
Allows settings FIDO2 options.

## SYNTAX

### Set PIN minimum length
```
Set-YubiKeyFIDO2 -MinimumPINLength <Int32> [<CommonParameters>]
```

### Set force PIN change
```
Set-YubiKeyFIDO2 [-ForcePINChange] [<CommonParameters>]
```

### Send MinimumPIN to RelyingParty
```
Set-YubiKeyFIDO2 -MinimumPINRelyingParty <String> [<CommonParameters>]
```

### Set PIN
```
Set-YubiKeyFIDO2 [-SetPIN] [-OldPIN <SecureString>] -NewPIN <SecureString> [<CommonParameters>]
```

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
Type: SwitchParameter
Parameter Sets: Set force PIN change
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MinimumPINLength
Set the minimum length of the PIN

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

### -MinimumPINRelyingParty
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

### -NewPIN
New PIN code to set for the FIDO2 module.

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

### -OldPIN
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

### -SetPIN
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

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
