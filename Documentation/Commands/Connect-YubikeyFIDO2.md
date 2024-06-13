---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Connect-YubikeyFIDO2

## SYNOPSIS
Connect to the FIDO2 session

## SYNTAX

```
Connect-YubikeyFIDO2 [-PIN] <String> [<CommonParameters>]
```

## DESCRIPTION
Allows FIDO2 commands to be sent to the Yubikey
Must be run as Administrator

## EXAMPLES

### Example 1
```powershell
PS C:\> connect-YubikeyFIDO2 -PIN (Read-Host -MaskInput -Prompt "PIN")
PIN: ******
```

Connect to the FIDO2 session without exposing the PIN code on the screen

## PARAMETERS

### -PIN
FIDO2 PIN

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
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
