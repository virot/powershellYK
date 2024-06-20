---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Set-Yubikey

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### Replace USB capabilities
```
Set-Yubikey -UsbCapabilities <YubiKeyCapabilities> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Update USB capabilities
```
Set-Yubikey [-EnableUsbCapabilities <YubiKeyCapabilities>] [-DisableUsbCapabilities <YubiKeyCapabilities>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Replace NFC capabilities
```
Set-Yubikey -NFCCapabilities <YubiKeyCapabilities> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Update NFC capabilities
```
Set-Yubikey [-EnableNFCCapabilities <YubiKeyCapabilities>] [-DisableNFCCapabilities <YubiKeyCapabilities>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Update Touch Eject flag
```
Set-Yubikey -TouchEject <Boolean> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Set automatically eject
```
Set-Yubikey -AutoEjectTimeout <UInt16> [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
{{ Fill in the Description }}

## EXAMPLES

### Example 1
```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -AutoEjectTimeout
Automatically eject after the given time.
Implies -TouchEject:$True.
Value in seconds.

```yaml
Type: UInt16
Parameter Sets: Set automatically eject
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DisableNFCCapabilities
Disable capabilities to NFC

```yaml
Type: YubiKeyCapabilities
Parameter Sets: Update NFC capabilities
Aliases:
Accepted values: None, Otp, FidoU2f, Ccid, OpenPgp, Piv, Oath, YubiHsmAuth, Fido2, All

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DisableUsbCapabilities
Disable capabilities to USB

```yaml
Type: YubiKeyCapabilities
Parameter Sets: Update USB capabilities
Aliases:
Accepted values: None, Otp, FidoU2f, Ccid, OpenPgp, Piv, Oath, YubiHsmAuth, Fido2, All

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -EnableNFCCapabilities
Enable capabilities to NFC

```yaml
Type: YubiKeyCapabilities
Parameter Sets: Update NFC capabilities
Aliases:
Accepted values: None, Otp, FidoU2f, Ccid, OpenPgp, Piv, Oath, YubiHsmAuth, Fido2, All

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -EnableUsbCapabilities
Enabled capabilities to USB

```yaml
Type: YubiKeyCapabilities
Parameter Sets: Update USB capabilities
Aliases:
Accepted values: None, Otp, FidoU2f, Ccid, OpenPgp, Piv, Oath, YubiHsmAuth, Fido2, All

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NFCCapabilities
Replace current NFC capabilities with.

```yaml
Type: YubiKeyCapabilities
Parameter Sets: Replace NFC capabilities
Aliases:
Accepted values: None, Otp, FidoU2f, Ccid, OpenPgp, Piv, Oath, YubiHsmAuth, Fido2, All

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -TouchEject
Allows loading/unloading the smartcard by touching the yubikey.

```yaml
Type: Boolean
Parameter Sets: Update Touch Eject flag
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UsbCapabilities
Replace current USB capabilities with.

```yaml
Type: YubiKeyCapabilities
Parameter Sets: Replace USB capabilities
Aliases:
Accepted values: None, Otp, FidoU2f, Ccid, OpenPgp, Piv, Oath, YubiHsmAuth, Fido2, All

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Confirm
Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WhatIf
Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

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
