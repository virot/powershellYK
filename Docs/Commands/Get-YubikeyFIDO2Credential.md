---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Get-YubikeyFIDO2Credential

## SYNOPSIS
Read the FIDO2 discoverable credentials

## SYNTAX

```
Get-YubikeyFIDO2Credential [<CommonParameters>]
```

## DESCRIPTION
Get what FIDO2 credentials that have been saved in the Yubikey.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-YubikeyFIDO2Credential

Site            Name         DisplayName
----            ----         -----------
demo.yubico.com powershellYK powershellYK
```

Lists all sites and usernames for all discoverable credentials.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
