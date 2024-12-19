---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Rename-YubikeyUVFingerprint

## SYNOPSIS
Changes the friendlyname of a fingerprint on the YubiKey Bio.

## SYNTAX

### Rename using Name
```
Rename-YubikeyUVFingerprint -Name <String> -NewName <String> [<CommonParameters>]
```

### Rename using ID
```
Rename-YubikeyUVFingerprint -ID <String> -NewName <String> [<CommonParameters>]
```

## DESCRIPTION
You can update the friendly name of a fingerprint on the yubikey Bio.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-YubikeyuVFingerprint

ID   Name
--   ----
FC04 tumme

PS C:\> Rename-YubikeyUVFingerprint -ID fc04 -NewName "Thumb"
```

Changes the friendly name of the finger with ID fc04 to "Thumb".

## PARAMETERS

### -ID
ID of finger to rename

```yaml
Type: String
Parameter Sets: Rename using ID
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
Friendly name of finger to rename

```yaml
Type: String
Parameter Sets: Rename using Name
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NewName
New friendly name

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
