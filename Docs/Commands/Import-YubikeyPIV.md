---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Import-YubiKeyPIV

## SYNOPSIS

Import certificate

## SYNTAX

### CertificateOnly

```
Import-YubiKeyPIV -Slot <PIVSlot> -Certificate <Object> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### CertificateAndKey

```
Import-YubiKeyPIV -Slot <PIVSlot> -Certificate <Object> -PrivateKeyPath <FileInfo>
 [-Password <SecureString>] [-PinPolicy <PivPinPolicy>] [-TouchPolicy <PivTouchPolicy>] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

### P12

```
Import-YubiKeyPIV -Slot <PIVSlot> -P12Path <FileInfo> [-Password <SecureString>]
 [-PinPolicy <PivPinPolicy>] [-TouchPolicy <PivTouchPolicy>] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### Privatekey

```
Import-YubiKeyPIV -Slot <PIVSlot> -PrivateKeyPath <FileInfo> [-Password <SecureString>]
 [-PinPolicy <PivPinPolicy>] [-TouchPolicy <PivTouchPolicy>] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Imports a certicate into the Yubikey

## EXAMPLES

### Example 1

```powershell
PS C:\> Import-YubikeyPIV -Slot 0x9a -Certificate certificate.cer
```

Import certificate.cer into the certificate slot 0x9a

### Example 2

```powershell
PS C:\> Import-YubikeyPIV -Slot "Digital Signature" -PrivateKeyPath .\ecc_384.pem -Password (Read-Host -AsSecureString "Password")
```

Import certificate.cer into the certificate slot 0x9a

## PARAMETERS

### -Certificate

Certificate to be stored

```yaml
Type: System.Object
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: CertificateOnly
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: CertificateAndKey
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

### -P12Path

P12 file to be stored

```yaml
Type: System.IO.FileInfo
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: P12
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Password

Private key password

```yaml
Type: System.Security.SecureString
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Privatekey
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: CertificateAndKey
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: P12
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PinPolicy

Pin policy

```yaml
Type: Yubico.YubiKey.Piv.PivPinPolicy
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Privatekey
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: CertificateAndKey
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: P12
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- Default
- Never
- None
- Once
HelpMessage: ''
```

### -PrivateKeyPath

Private key to be stored

```yaml
Type: System.IO.FileInfo
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Privatekey
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: CertificateAndKey
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Slot

Slot number

```yaml
Type: powershellYK.PIV.PIVSlot
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -TouchPolicy

Touch policy

```yaml
Type: Yubico.YubiKey.Piv.PivTouchPolicy
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Privatekey
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: CertificateAndKey
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: P12
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues:
- Default
- Never
- Always
- Cached
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

