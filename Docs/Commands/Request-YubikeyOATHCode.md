---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Request-YubikeyOATHCode

## SYNOPSIS
Displays TOTP / HOTP codes for YubiKey OATH credentials.

## SYNTAX

### All (Default)
```
Request-YubikeyOATHCode [-All] [<CommonParameters>]
```

### Specific
```
Request-YubikeyOATHCode -Account <Credential> [<CommonParameters>]
```

## DESCRIPTION
Displays TOTP / HOTP codes for Yubikey OATH credentials

## EXAMPLES

### Example 1
```powershell
PS C:\> Request-YubikeyOATHCode

                                 Value  validFrom           validUntil
              Name

-------------------------------- -----  ---------           ----------
Issuer Issuer:dsa                221624 2024-06-15 19:26:00 2024-06-15 19:26:30
```

List the current code for all OATH credentials

## PARAMETERS

### -Account
Account to generate code for

```yaml
Type: Credential
Parameter Sets: Specific
Aliases: Credential

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -All
Get codes for all credentials

```yaml
Type: SwitchParameter
Parameter Sets: All
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Yubico.YubiKey.Oath.Credential

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
