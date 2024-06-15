---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Set-YubikeyOATHAccount

## SYNOPSIS
Update OATH account

## SYNTAX

```
Set-YubikeyOATHCredential -Credential <Credential> [-NewAccountName <String>] [-NewIssuer <String>]
 [<CommonParameters>]
```

## DESCRIPTION
Update OATH account

## EXAMPLES

### Example 1
```powershell
PS C:\> $Accounttochange = Get-YubikeyOATHAccount | Where-Object {$_.Issuer -eq 'Yubico Demo'}
PS C:\> Set-YubikeyOATHAccount -Credential $Accounttochange -NewIssuer "powershellYK Demo"
```

Selects and updates the Issuer from 'Yubico Demo' to 'powershellYK Demo' 

## PARAMETERS

### -Account
Account to remove

```yaml
Type: Credential
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NewAccountName
New AccountName

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

### -NewIssuer
New Issuer

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
