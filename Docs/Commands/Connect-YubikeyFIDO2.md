---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Connect-YubikeyFIDO2

## SYNOPSIS
Connect to the FIDO2 session.

## SYNTAX

```
Connect-YubikeyFIDO2 -PIN <SecureString> [<CommonParameters>]
```

## DESCRIPTION
Allows FIDO2 commands to be sent to the YubiKey
Must be run as Administrator

## EXAMPLES

### Example 1
```powershell
PS C:\> Connect-YubikeyFIDO2

cmdlet Connect-YubikeyFIDO2 at command pipeline position 1
Supply values for the following parameters:
(Type !? for Help.)
PIN: ******
```

Connect to FIDO2 module with default ManagmentKey

### Example 2
```powershell
PS C:\> $PIN = Read-Host -AsSecureString 'PIN'
PIN: ******
PS C:\> Connect-YubikeyFIDO2 -PIN $PIN
```

Connect to FIDO2 module with default Managementkey and a stored pin requested from the commandline

### Example 3
```powershell
PS C:\> $PIN = ConvertTo-SecureString -String "123456" -AsPlainText -Force
PS C:\> Connect-YubikeyFIDO2 -PIN $PIN
```

Connect to FIDO2 module with default Managementkey and a stored pin requested constructed from code

## PARAMETERS

### -PIN
FIDO2 PIN

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
