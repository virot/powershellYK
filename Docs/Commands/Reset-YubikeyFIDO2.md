---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Reset-YubikeyFIDO2

## SYNOPSIS
Reset a Yubikey FIDO2 device to factory settings.

## SYNTAX

```
Reset-YubikeyFIDO2 [<CommonParameters>]
```

## DESCRIPTION
Resets a Yubikey FIDO2 device to factory settings. This will remove all stored credentials and reset the device to factory settings.
This REQUIRES the yubikey to be inserted at a maximum 5 seconds before running this command.

## EXAMPLES

### Example 1
```powershell
PS C:\> Reset-YubikeyFIDO2
```

Resets the Yubikey FIDO2 device to factory settings.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
