---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Set-YubikeyFIDO2

## SYNOPSIS
Allows settings FIDO2 options.

## SYNTAX

### Set PIN minimum length
```
Set-YubikeyFIDO2 -MinimumPINLength <Int32> [<CommonParameters>]
```

### Set force PIN change
```
Set-YubikeyFIDO2 -ForcePINChange <Boolean> [<CommonParameters>]
```

### Send MinimumPIN to RelyingParty
```
Set-YubikeyFIDO2 -MinimumPINRelyingParty <String> [<CommonParameters>]
```

### Set PIN
```
Set-YubikeyFIDO2 [-SetPIN] [-OldPIN <SecureString>] -NewPIN <SecureString> [<CommonParameters>]
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

To change the PIN code make sure to connect to the Yubikey first.

## PARAMETERS

### -ForcePINChange
Enable or disable the forceChangePin flag.

```yaml
Type: Boolean
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
