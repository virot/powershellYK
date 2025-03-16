---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
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
Import-YubiKeyPIV -Slot <PIVSlot> -Certificate <Object> -PrivateKeyPath <FileInfo> [-Password <SecureString>]
 [-PinPolicy <PivPinPolicy>] [-TouchPolicy <PivTouchPolicy>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### P12
```
Import-YubiKeyPIV -Slot <PIVSlot> -P12Path <FileInfo> [-Password <SecureString>] [-PinPolicy <PivPinPolicy>]
 [-TouchPolicy <PivTouchPolicy>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Privatekey
```
Import-YubiKeyPIV -Slot <PIVSlot> -PrivateKeyPath <FileInfo> [-Password <SecureString>]
 [-PinPolicy <PivPinPolicy>] [-TouchPolicy <PivTouchPolicy>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

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
Type: Object
Parameter Sets: CertificateOnly, CertificateAndKey
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -P12Path
P12 file to be stored

```yaml
Type: FileInfo
Parameter Sets: P12
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Password
Private key password

```yaml
Type: SecureString
Parameter Sets: CertificateAndKey, P12, Privatekey
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PinPolicy
PinPolicy

```yaml
Type: PivPinPolicy
Parameter Sets: CertificateAndKey, P12, Privatekey
Aliases:
Accepted values: Default, Never, None, Once

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PrivateKeyPath
Private key to be stored

```yaml
Type: FileInfo
Parameter Sets: CertificateAndKey, Privatekey
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Slot
Slotnumber

```yaml
Type: PIVSlot
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -TouchPolicy
TouchPolicy

```yaml
Type: PivTouchPolicy
Parameter Sets: CertificateAndKey, P12, Privatekey
Aliases:
Accepted values: Default, Never, Always, Cached

Required: False
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
Shows what would happen if the cmdlet runs. The cmdlet is not run.

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
