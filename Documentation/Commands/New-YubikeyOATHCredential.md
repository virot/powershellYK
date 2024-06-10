---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# New-YubikeyOATHCredential

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### HOTP
```
New-YubikeyOATHCredential [-HOTP] -Issuer <String> -Accountname <String> [-Algorithm <HashAlgorithm>]
 -Secret <String> [-Digits <Int32>] [-Counter] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### TOTP
```
New-YubikeyOATHCredential [-TOTP] -Issuer <String> -Accountname <String> [-Algorithm <HashAlgorithm>]
 -Secret <String> -Period <CredentialPeriod> [-Digits <Int32>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
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

### -Accountname
Accountname

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Algorithm
Algorithm

```yaml
Type: HashAlgorithm
Parameter Sets: (All)
Aliases:
Accepted values: SHA1, SHA256, SHA512

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Counter
Counter

```yaml
Type: SwitchParameter
Parameter Sets: HOTP
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Digits
Digits

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -HOTP
Type of OATH

```yaml
Type: SwitchParameter
Parameter Sets: HOTP
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Issuer
Issuer

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Period
Period for credential

```yaml
Type: CredentialPeriod
Parameter Sets: TOTP
Aliases:
Accepted values: Undefined, Period15, Period30, Period60

Required: True
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

### -Secret
Secret

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -TOTP
Type of OATH

```yaml
Type: SwitchParameter
Parameter Sets: TOTP
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
