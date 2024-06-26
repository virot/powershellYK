﻿---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Set-YubikeyPIV

## SYNOPSIS
Allows the updating of PIV settings

## SYNTAX

### ChangeRetries
```
Set-YubikeyPIV -PinRetries <Byte> -PukRetries <Byte> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### ChangePIN
```
Set-YubikeyPIV -PIN <SecureString> -NewPIN <SecureString> [-ChangePIN] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### UnblockPIN
```
Set-YubikeyPIV -NewPIN <SecureString> -PUK <SecureString> [-UnblockPIN] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### ChangePUK
```
Set-YubikeyPIV -PUK <SecureString> -NewPUK <SecureString> [-ChangePUK] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### ChangeManagement
```
Set-YubikeyPIV -ManagementKey <PSObject> -NewManagementKey <PSObject> -Algorithm <PivAlgorithm>
 -TouchPolicy <PivTouchPolicy> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### newCHUID
```
Set-YubikeyPIV [-newCHUID] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Set Managementkey to PIN protected
```
Set-YubikeyPIV [-PINProtectedManagementkey] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Allows the modification of PIV settings like: PIN, PUK, ManagementKey and CHUID.

## EXAMPLES

### Example 1
```powershell
PS C:\> Set-YubikeyPIV -PinRetries 8 -PukRetries 4
WARNING: PIN and PUK codes reset to default, remember to change.
```

Updates the PIV to 8 PIN retries and 4 PUK retries.

### Example 3
```powershell
PS C:\GIT-VS\Yubikey_Powershell> Set-YubikeyPIV -ChangePIN

cmdlet Set-YubikeyPIV at command pipeline position 1
Supply values for the following parameters:
(Type !? for Help.)
PIN: ******
NewPIN: ******
```

Change PIN with a easy way of requesting the new codes.

## PARAMETERS

### -Algorithm
Algoritm

```yaml
Type: PivAlgorithm
Parameter Sets: ChangeManagement
Aliases:
Accepted values: TripleDES, AES128, AES192, AES256

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ChangePIN
Easy access to ChangePIN

```yaml
Type: SwitchParameter
Parameter Sets: ChangePIN
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ChangePUK
Easy access to ChangePUK

```yaml
Type: SwitchParameter
Parameter Sets: ChangePUK
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ManagementKey
Current ManagementKey

```yaml
Type: PSObject
Parameter Sets: ChangeManagement
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -newCHUID
Generate new CHUID

```yaml
Type: SwitchParameter
Parameter Sets: newCHUID
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NewManagementKey
New ManagementKey

```yaml
Type: PSObject
Parameter Sets: ChangeManagement
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NewPIN
New PIN

```yaml
Type: SecureString
Parameter Sets: ChangePIN, UnblockPIN
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NewPUK
New PUK

```yaml
Type: SecureString
Parameter Sets: ChangePUK
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PIN
Current PIN

```yaml
Type: SecureString
Parameter Sets: ChangePIN
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PINProtectedManagementkey
PIN protect the Managementkey

```yaml
Type: SwitchParameter
Parameter Sets: Set Managementkey to PIN protected
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PinRetries
{{ Fill PinRetries Description }}

```yaml
Type: Byte
Parameter Sets: ChangeRetries
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PUK
Current PUK

```yaml
Type: SecureString
Parameter Sets: UnblockPIN, ChangePUK
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PukRetries
{{ Fill PukRetries Description }}

```yaml
Type: Byte
Parameter Sets: ChangeRetries
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -TouchPolicy
TouchPolicy

```yaml
Type: PivTouchPolicy
Parameter Sets: ChangeManagement
Aliases:
Accepted values: Default, Never, Always, Cached

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UnblockPIN
Easy access to UnblockPIN

```yaml
Type: SwitchParameter
Parameter Sets: UnblockPIN
Aliases:

Required: False
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
Shows what would happen if the cmdlet runs. The cmdlet is not run.

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
