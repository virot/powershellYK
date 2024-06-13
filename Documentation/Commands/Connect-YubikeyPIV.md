---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Connect-YubikeyPIV

## SYNOPSIS
Connect PIV module

## SYNTAX

```
Connect-YubikeyPIV [-ManagementKey <String>] -PIN <SecureString> [<CommonParameters>]
```

## DESCRIPTION
Connects the PIV module for the currently connected Yubikey, with PIN and Managementkey as needed

## EXAMPLES

### Example 1
```powershell
PS C:\> Connect-YubikeyPIV

cmdlet Connect-YubikeyPIV at command pipeline position 1
Supply values for the following parameters:
(Type !? for Help.)
PIN: ******
```

Connect to PIV module with default ManagmentKey

### Example 2
```powershell
PS C:\> $PIN = Read-Host -AsSecureString 'PIN'
PIN: ******
PS C:\> Connect-YubikeyPIV -PIN $PIN
```

Connect to PIV module with default Managementkey and a stored pin requested from the commandline

### Example 3
```powershell
PS C:\> $PIN = ConvertTo-SecureString -String "123456" -AsPlainText -Force
PS C:\> Connect-YubikeyPIV -PIN $PIN
```

Connect to PIV module with default Managementkey and a stored pin requested constructed from code

## PARAMETERS

### -ManagementKey
ManagementKey

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PIN
PIN

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
