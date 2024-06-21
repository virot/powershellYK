---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Protect-YubikeyOATH

## SYNOPSIS
Set / update password

## SYNTAX

```
Protect-YubikeyOATH -UpdatePassword <SecureString> [<CommonParameters>]
```

## DESCRIPTION
Set / update the password for the Yubikey OATH application.

## EXAMPLES

### Example 1
```powershell
PS C:\> Protect-YubikeyOATH
UpdatePassword: ********
```

Secures the OATH application

## PARAMETERS

### -UpdatePassword
Password

```yaml
Type: SecureString
Parameter Sets: (All)
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

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
