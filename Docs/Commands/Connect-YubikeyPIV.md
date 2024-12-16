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

### PIN (Default)
```
Connect-YubikeyPIV -PIN <SecureString> [<CommonParameters>]
```

### PIN&Management
```
Connect-YubikeyPIV -ManagementKey <PSObject> -PIN <SecureString> [<CommonParameters>]
```

### Management
```
Connect-YubikeyPIV -ManagementKey <PSObject> [<CommonParameters>]
```

## DESCRIPTION
Connects the PIV module for the currently connected YubiKey, with PIN and Management Key as needed

## EXAMPLES

### Example 1
```powershell
PS C:\> Connect-YubikeyPIV

cmdlet Connect-YubikeyPIV at command pipeline position 1
Supply values for the following parameters:
(Type !? for Help.)
PIN: ******
```

Connect to PIV module with default Managment Key

### Example 2
```powershell
PS C:\> $PIN = Read-Host -AsSecureString 'PIN'
PIN: ******
PS C:\> Connect-YubikeyPIV -PIN $PIN
```

Connect to PIV module with default Management key and a stored pin requested from the command line

### Example 3
```powershell
PS C:\> $PIN = ConvertTo-SecureString -String "123456" -AsPlainText -Force
PS C:\> Connect-YubikeyPIV -PIN $PIN
```

Connect to PIV module with default Managementkey and a stored PIN requested constructed from code

## PARAMETERS

### -ManagementKey
ManagementKey

```yaml
Type: PSObject
Parameter Sets: PIN&Management, Management
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PIN
PIN

```yaml
Type: SecureString
Parameter Sets: PIN, PIN&Management
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
