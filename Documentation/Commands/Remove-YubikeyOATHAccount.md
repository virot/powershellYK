---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Remove-YubikeyOATHAccount

## SYNOPSIS
Removes an account from the Yubikey OATH application

## SYNTAX

```
Remove-YubikeyOATHAccount -Account <Credential> [<CommonParameters>]
```

## DESCRIPTION
Removes an account from the Yubikey OATH application

## EXAMPLES

### Example 1
```powershell
PS C:\> Remove-YubikeyOATHAccount -Account (Get-YubikeyOATHAccount | ?{$_.Issuer -eq 'Yubico Demo'})
```

Removes the account with the issuer 'Yubico Demo'.

## PARAMETERS

### -Credential
Credential to remove

```yaml
Type: Credential
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
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
