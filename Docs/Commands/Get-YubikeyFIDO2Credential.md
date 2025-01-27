---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Get-YubiKeyFIDO2Credential

## SYNOPSIS
Read the FIDO2 discoverable credentials

## SYNTAX

### List-All (Default)
```
Get-YubiKeyFIDO2Credential [<CommonParameters>]
```

### List-CredentialID
```
Get-YubiKeyFIDO2Credential -CredentialID <CredentialID> [<CommonParameters>]
```

### List-CredentialID-Base64URL
```
Get-YubiKeyFIDO2Credential -CredentialIdBase64Url <String> [<CommonParameters>]
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

### -CredentialID
Credential ID to remove

```yaml
Type: CredentialID
Parameter Sets: List-CredentialID
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -CredentialIdBase64Url
Credential ID to remove int Base64 URL encoded format

```yaml
Type: String
Parameter Sets: List-CredentialID-Base64URL
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
