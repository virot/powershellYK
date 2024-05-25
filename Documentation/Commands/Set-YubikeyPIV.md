---
external help file: VirotYubikey.dll-Help.xml
Module Name: VirotYubikey
online version:
schema: 2.0.0
---

# Set-YubikeyPIV

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### ChangeRetries
```
Set-YubikeyPIV -PinRetries <Byte> -PukRetries <Byte> [-PIN <String>] [-NewPIN <String>] [-PUK <String>]
 [-NewPUK <String>] [-ManagementKey <String>] [-NewManagementKey <String>] [-Algorithm <PivAlgorithm>]
 [-TouchPolicy <PivTouchPolicy>] [-newCHUID] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### ChangePIN
```
Set-YubikeyPIV [-PinRetries <Byte>] [-PukRetries <Byte>] -PIN <String> -NewPIN <String> [-PUK <String>]
 [-NewPUK <String>] [-ManagementKey <String>] [-NewManagementKey <String>] [-Algorithm <PivAlgorithm>]
 [-TouchPolicy <PivTouchPolicy>] [-newCHUID] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### UnblockPIN
```
Set-YubikeyPIV [-PinRetries <Byte>] [-PukRetries <Byte>] [-PIN <String>] -NewPIN <String> -PUK <String>
 [-NewPUK <String>] [-ManagementKey <String>] [-NewManagementKey <String>] [-Algorithm <PivAlgorithm>]
 [-TouchPolicy <PivTouchPolicy>] [-newCHUID] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### ChangePUK
```
Set-YubikeyPIV [-PinRetries <Byte>] [-PukRetries <Byte>] [-PIN <String>] [-NewPIN <String>] -PUK <String>
 -NewPUK <String> [-ManagementKey <String>] [-NewManagementKey <String>] [-Algorithm <PivAlgorithm>]
 [-TouchPolicy <PivTouchPolicy>] [-newCHUID] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### ChangeManagement
```
Set-YubikeyPIV [-PinRetries <Byte>] [-PukRetries <Byte>] [-PIN <String>] [-NewPIN <String>] [-PUK <String>]
 [-NewPUK <String>] -ManagementKey <String> -NewManagementKey <String> -Algorithm <PivAlgorithm>
 -TouchPolicy <PivTouchPolicy> [-newCHUID] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### newCHUID
```
Set-YubikeyPIV [-PinRetries <Byte>] [-PukRetries <Byte>] [-PIN <String>] [-NewPIN <String>] [-PUK <String>]
 [-NewPUK <String>] [-ManagementKey <String>] [-NewManagementKey <String>] [-Algorithm <PivAlgorithm>]
 [-TouchPolicy <PivTouchPolicy>] [-newCHUID] [-ProgressAction <ActionPreference>] [<CommonParameters>]
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

### -Algorithm
Algoritm

```yaml
Type: PivAlgorithm
Parameter Sets: ChangeRetries, ChangePIN, UnblockPIN, ChangePUK, newCHUID
Aliases:
Accepted values: TripleDES, AES128, AES192, AES256

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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
Parameter Sets: ChangeRetries, ChangePIN, UnblockPIN, ChangePUK, newCHUID
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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
Parameter Sets: ChangeRetries, ChangePIN, UnblockPIN, ChangePUK, ChangeManagement
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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
Parameter Sets: ChangeRetries, ChangePIN, UnblockPIN, ChangePUK, newCHUID
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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
Parameter Sets: ChangeRetries, ChangePUK, ChangeManagement, newCHUID
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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
Parameter Sets: ChangeRetries, ChangePIN, UnblockPIN, ChangeManagement, newCHUID
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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
Parameter Sets: ChangeRetries, UnblockPIN, ChangePUK, ChangeManagement, newCHUID
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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

```yaml
Type: Byte
Parameter Sets: ChangePIN, UnblockPIN, ChangePUK, ChangeManagement, newCHUID
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PUK
Current PUK

```yaml
Type: String
Parameter Sets: ChangeRetries, ChangePIN, ChangeManagement, newCHUID
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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

```yaml
Type: Byte
Parameter Sets: ChangePIN, UnblockPIN, ChangePUK, ChangeManagement, newCHUID
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -TouchPolicy
TouchPolicy

```yaml
Type: PivTouchPolicy
Parameter Sets: ChangeRetries, ChangePIN, UnblockPIN, ChangePUK, newCHUID
Aliases:
Accepted values: Default, Never, Always, Cached

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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
