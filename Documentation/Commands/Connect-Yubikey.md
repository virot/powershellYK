---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Connect-Yubikey

## SYNOPSIS
Connect the module to the Yubikey

## SYNTAX

```
Connect-Yubikey [[-YubiKey] <YubiKeyDevice>] [<CommonParameters>]
```

## DESCRIPTION
The `Connect-Yubikey` cmdlet allows the module connect to a yubikey. The command allows specific Yubikey to be connected.

## EXAMPLES

### Example 1
```powershell
PS C:\> Connect-Yubikey
```

Try to connect to a single Yubikey, will fail if number of connected Yubikeys aren't one.

### Example 2
```powershell
PS C:\> Connect-Yubikey -YubiKey (Find-Yubikey -Serialnumber 12345)
```

Connect to a specific Yubikey with serial 12345

## PARAMETERS

### -YubiKey
Which yubikey to connect to

```yaml
Type: YubiKeyDevice
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Yubico.YubiKey.YubiKeyDevice

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
