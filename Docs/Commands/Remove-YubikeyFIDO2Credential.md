---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Remove-YubikeyFIDO2Credential

## SYNOPSIS
Removes a FIDO2 credential from the YubiKey.

## SYNTAX

### Remove with CredentialID (Default)
```
Remove-YubikeyFIDO2Credential -CredentialId <CredentialID> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Remove with username and RelayingParty
```
Remove-YubikeyFIDO2Credential -Username <String> -RelayingParty <String> [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## DESCRIPTION
Allows the removal of a FIDO2 credential from the YubiKey. The credential can be removed by specifying the CredentialID or by specifying the Username and RelayingParty.
The Cmdlet also allows piping of the CredentialID to remove the credential.

## EXAMPLES

### Example 1
```powershell
PS C:\> Remove-YubikeyFIDO2Credential -User 'powershellYK' -RelayingParty 'demo.yubico.com'
```

Removes the credential for the user 'powershellYK' from the RelayingParty 'demo.yubico.com'

### Example 2
```powershell
PS C:\> Remove-YubikeyFIDO2Credential -CredentialId ac37c06c15ec4458d0cf545db3cc0f8e3992e512d1c3e19d571417b12124634f01e6e3397bdbc8e74b96f950ea4bf600
```

Removes the credential with a specified CredentialID 

### Example 3
```powershell
PS C:\> Get-YubiKeyFIDO2Credential|Where-Object RPId -eq 'demo.yubico.com'|Remove-YubikeyFIDO2Credential -Confirm:$false
```

Removes all FIDO2 credentials for the RelayingParty 'demo.yubico.com'

## PARAMETERS

### -CredentialId
Credential ID to remove

```yaml
Type: CredentialID
Parameter Sets: Remove with CredentialID
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -RelayingParty
RelayingParty to remove user from

```yaml
Type: String
Parameter Sets: Remove with username and RelayingParty
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Username
User to remove

```yaml
Type: String
Parameter Sets: Remove with username and RelayingParty
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Confirm
Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WhatIf
Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### powershellYK.FIDO2.CredentialID

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
