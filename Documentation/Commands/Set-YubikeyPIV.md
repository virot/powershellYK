---
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
Set-YubikeyPIV -PinRetries <Byte> -PukRetries <Byte> [<CommonParameters>]
```

### ChangePIN
```
Set-YubikeyPIV -PIN <String> -NewPIN <String> [<CommonParameters>]
```

### UnblockPIN
```
Set-YubikeyPIV -NewPIN <String> -PUK <String> [<CommonParameters>]
```

### ChangePUK
```
Set-YubikeyPIV -PUK <String> -NewPUK <String> [<CommonParameters>]
```

### ChangeManagement
```
Set-YubikeyPIV -ManagementKey <String> -NewManagementKey <String> -Algorithm <PivAlgorithm>
 -TouchPolicy <PivTouchPolicy> [<CommonParameters>]
```

### newCHUID
```
Set-YubikeyPIV [-newCHUID] [<CommonParameters>]
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

### Example 2
```powershell
PS C:\> Set-YubikeyPIV -newCHUID
```

Creates a new CHUID to allow windows to discover the yubikey as a new smartcard.

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

### -ManagementKey
Current ManagementKey

```yaml
Type: String
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
Type: String
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
Type: String
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
Type: String
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
Type: String
Parameter Sets: ChangePIN
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
Type: String
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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
