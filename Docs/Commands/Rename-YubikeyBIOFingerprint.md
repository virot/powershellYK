---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# Rename-YubiKeyBIOFingerprint

## SYNOPSIS
Changes the template name of a registered fingerprint on the YubiKey Bio or YubiKey Bio Multi-Protocol Edition (MPE).

## SYNTAX

### Rename using Name
```
Rename-YubiKeyBIOFingerprint -Name <String> -NewName <String> [<CommonParameters>]
```

### Rename using ID
```
Rename-YubiKeyBIOFingerprint -ID <String> -NewName <String> [<CommonParameters>]
```

## DESCRIPTION
You can update the friendly name of a fingerprint on the yubikey Bio.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-YubikeyBIOFingerprint

ID   Name
--   ----
23FC left index

PS C:\> Rename-YubikeyBIOFingerprint -ID 23FC -NewName "left index finger"
Fingerprint renamed (left index finger).
```

Changes the friendly name of the fingerprint with name "left index" to "left index finger".

### Example 2
```powershell
PS C:\> Get-YubikeyBIOFingerprint

ID   Name
--   ----
23FC left index

PS C:\> Rename-YubikeyBIOFingerprint -Name "left" -NewName "thumb"
Fingerprint renamed (thumb).
```

Changes the friendly name of the fingerprint with name "left index to "thumb".

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
