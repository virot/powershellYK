---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Set-Yubikey

## SYNOPSIS
Allows basic YubiKey configuration.

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

### Set Restricted NFC
```
Set-Yubikey [-SecureTransportMode] [-WhatIf] [-Confirm] [<CommonParameters>]
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
Allows configuration of USB / NFC capabilities and the touch eject flag.

## EXAMPLES

### Example 1
```powershell
PS C:\> Set-Yubikey -UsbCapabilities All
WARNING: Yubikey will reboot, diconnecting powershellYK.
```

Enables all applications over USB.

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
Disable select capabilities over NFC. The command can be used to improve
user experience by _disabling__ YubiKey features that are not in use. 
For example, an organization may want to disable OTP/OATH if only FIDO or PIV is used.

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
Disable select capabilities over USB.  The command can be used to improve
user experience by _disabling__ YubiKey features that are not in use. 
For example, an organization may want to disable OTP/OATH if only FIDO or PIV is used.

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
Enable select capabilities over NFC. If a needed feature has been turned off,
the command can be used to (re)enable the feature over NFC.

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
Enable select capabilities over USB. If a needed feature has been turned off,
the command can be used to (re)enable the feature over USB.

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
Replace current NFC capabilities with selected capabilities.

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

### -SecureTransportMode
Enable Restricted NFC as supported by YubiKeys with firmware `5.7` or later.
When set, the YubiKey will limit access to capabilites over NFC until USB powered. 
This feature is typically toggled when _shipping__ YubiKeys in tamper-evident packaging.

```yaml
Type: SwitchParameter
Parameter Sets: Set Restricted NFC
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -TouchEject
Allows loading/unloading the smartcard by touching the YubiKey.

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
Replace current USB capabilities with with selected capabilities.

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
