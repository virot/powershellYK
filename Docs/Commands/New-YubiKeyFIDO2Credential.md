---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# New-YubiKeyFIDO2Credential

## SYNOPSIS
Creates a new FIDO2 credential on the connected YubiKey.
For more complete examples see: https://github.com/virot/powershellYK/tree/master/Docs/Examples

## SYNTAX

### UserEntity-HostData
```
New-YubiKeyFIDO2Credential -RelyingPartyID <String> [-RelyingPartyName <String>] -Challenge <Challenge>
 [-Discoverable <Boolean>] -UserEntity <UserEntity> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### UserData-RelyingParty
```
New-YubiKeyFIDO2Credential -RelyingParty <RelyingParty> -Challenge <Challenge> [-Discoverable <Boolean>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### UserEntity-RelyingParty
```
New-YubiKeyFIDO2Credential -RelyingParty <RelyingParty> -Challenge <Challenge> [-Discoverable <Boolean>]
 -UserEntity <UserEntity> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### UserData-HostData
```
New-YubiKeyFIDO2Credential -Username <String> [-UserDisplayName <String>] [-UserID <Byte[]>]
 -Challenge <Challenge> [-Discoverable <Boolean>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
{{ Fill in the Description }}

## EXAMPLES

### Example 1
```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -Challenge
Challange.

```yaml
Type: Challenge
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Discoverable
Should this credential be discoverable.

```yaml
Type: Boolean
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -RelyingParty
RelaingParty object.

```yaml
Type: RelyingParty
Parameter Sets: UserData-RelyingParty, UserEntity-RelyingParty
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -RelyingPartyID
Specify which relayingParty (site) this credential is regards to.

```yaml
Type: String
Parameter Sets: UserEntity-HostData
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -RelyingPartyName
Friendlyname for the relayingParty.

```yaml
Type: String
Parameter Sets: UserEntity-HostData
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UserDisplayName
UserDisplayName to create credental for.

```yaml
Type: String
Parameter Sets: UserData-HostData
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UserEntity
Supply the user entity in complete form.

```yaml
Type: UserEntity
Parameter Sets: UserEntity-HostData, UserEntity-RelyingParty
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UserID
UserID.

```yaml
Type: Byte[]
Parameter Sets: UserData-HostData
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Username
Username to create credental for.

```yaml
Type: String
Parameter Sets: UserData-HostData
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

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS

[Enroll YubiKey FIDO2 against demo.yubico.com](https://github.com/virot/powershellYK/blob/master/Docs/Examples/Enroll%20YubiKey%20FIDO2%20against%20demo.yubico.com.md)
[Enroll YubiKey FIDO2 against login.microsoft.com](https://github.com/virot/powershellYK/blob/master/Docs/Examples/Enroll%20YubiKey%20FIDO2%20against%20login.microsoft.com.md)