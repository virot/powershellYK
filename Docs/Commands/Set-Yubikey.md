---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Set-Yubikey

## SYNOPSIS

Allows basic YubiKey configuration.

## SYNTAX

### Replace USB capabilities

```
Set-YubiKey -UsbCapabilities <YubiKeyCapabilities> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Update USB capabilities

```
Set-YubiKey [-EnableUsbCapabilities <YubiKeyCapabilities>]
 [-DisableUsbCapabilities <YubiKeyCapabilities>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Replace NFC capabilities

```
Set-YubiKey -NFCCapabilities <YubiKeyCapabilities> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Update NFC capabilities

```
Set-YubiKey [-EnableNFCCapabilities <YubiKeyCapabilities>]
 [-DisableNFCCapabilities <YubiKeyCapabilities>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Set Restricted NFC

```
Set-YubiKey -SecureTransportMode [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Update Touch Eject flag

```
Set-YubiKey -TouchEject <bool> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Set automatically eject

```
Set-YubiKey -AutoEjectTimeout <ushort> [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

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

Automatically eject after the given time. Implies -TouchEject:$True. Value in seconds.

```yaml
Type: System.UInt16
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Set automatically eject
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Confirm

Prompts you for confirmation before running the cmdlet.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases:
- cf
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -DisableNFCCapabilities

Disable select capabilities over NFC. The command can be used to improve
user experience by _disabling__ YubiKey features that are not in use.
For example, an organization may want to disable OTP/OATH if only FIDO or PIV is used.


```yaml
Type: Yubico.YubiKey.YubiKeyCapabilities
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Update NFC capabilities
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- None
- Otp
- FidoU2f
- Ccid
- OpenPgp
- Piv
- Oath
- YubiHsmAuth
- Fido2
- All
HelpMessage: ''
```

### -DisableUsbCapabilities

Disable select capabilities over USB.  The command can be used to improve
user experience by _disabling__ YubiKey features that are not in use.
For example, an organization may want to disable OTP/OATH if only FIDO or PIV is used.

```yaml
Type: Yubico.YubiKey.YubiKeyCapabilities
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Update USB capabilities
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- None
- Otp
- FidoU2f
- Ccid
- OpenPgp
- Piv
- Oath
- YubiHsmAuth
- Fido2
- All
HelpMessage: ''
```

### -EnableNFCCapabilities

Enable select capabilities over NFC. If a needed feature has been turned off,
the command can be used to (re)enable the feature over NFC.


```yaml
Type: Yubico.YubiKey.YubiKeyCapabilities
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Update NFC capabilities
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- None
- Otp
- FidoU2f
- Ccid
- OpenPgp
- Piv
- Oath
- YubiHsmAuth
- Fido2
- All
HelpMessage: ''
```

### -EnableUsbCapabilities

Enable select capabilities over USB. If a needed feature has been turned off,
the command can be used to (re)enable the feature over USB.


```yaml
Type: Yubico.YubiKey.YubiKeyCapabilities
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Update USB capabilities
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- None
- Otp
- FidoU2f
- Ccid
- OpenPgp
- Piv
- Oath
- YubiHsmAuth
- Fido2
- All
HelpMessage: ''
```

### -NFCCapabilities

Replace current NFC capabilities with selected capabilities.


```yaml
Type: System.Nullable`1[Yubico.YubiKey.YubiKeyCapabilities]
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Replace NFC capabilities
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- None
- Otp
- FidoU2f
- Ccid
- OpenPgp
- Piv
- Oath
- YubiHsmAuth
- Fido2
- All
HelpMessage: ''
```

### -SecureTransportMode

Enable Restricted NFC as supported by YubiKeys with firmware `5.7` or later.
When set, the YubiKey will limit access to capabilites over NFC until USB powered.
This feature is typically toggled when _shipping__ YubiKeys in tamper-evident packaging.


```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Set Restricted NFC
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -TouchEject

Allows loading/unloading the smartcard by touching the YubiKey.

```yaml
Type: System.Boolean
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Update Touch Eject flag
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -UsbCapabilities

Replace current USB capabilities with with selected capabilities.


```yaml
Type: Yubico.YubiKey.YubiKeyCapabilities
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Replace USB capabilities
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- None
- Otp
- FidoU2f
- Ccid
- OpenPgp
- Piv
- Oath
- YubiHsmAuth
- Fido2
- All
HelpMessage: ''
```

### -WhatIf


Runs the command in a mode that only reports what would happen without performing the actions.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases:
- wi
ParameterSets:
- Name: (All)
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

