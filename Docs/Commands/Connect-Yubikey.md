---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Connect-Yubikey

## SYNOPSIS
Connect the module to the YubiKey.

## SYNTAX

### Connect single Yubikey (Default)
```
Connect-Yubikey [<CommonParameters>]
```

### Connect provided Yubikey
```
Connect-Yubikey [[-YubiKey] <YubiKeyDevice>] [<CommonParameters>]
```

### Connect Yubikey with Serialnumber
```
Connect-Yubikey [-Serialnumber <Int32>] [<CommonParameters>]
```

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
Type: Int32
Parameter Sets: Connect Yubikey with Serialnumber
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -YubiKey
Which YubiKey to connect to

```yaml
Type: YubiKeyDevice
Parameter Sets: Connect provided Yubikey
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
