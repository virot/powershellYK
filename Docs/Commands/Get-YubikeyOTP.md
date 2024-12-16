---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Get-YubikeyOTP

## SYNOPSIS
YubiKey OTP Information

## SYNTAX

```
Get-YubikeyOTP [<CommonParameters>]
```

## DESCRIPTION
Command to retrive information about the YubiKey OTP configuration. As the Yubikey OTP does not allow information about what the slots are confgured to contain, this is not listed.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-YubikeyOTP

Slot       Configured RequiresTouch
----       ---------- -------------
ShortPress       True          True
LongPress        True          True
```

This command will display the current configuration of the Yubikey OTP.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
