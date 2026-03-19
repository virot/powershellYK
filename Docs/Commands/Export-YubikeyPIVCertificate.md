---
document type: cmdlet
external help file: powershellYK.dll-Help.xml
HelpUri: 
Module Name: powershellYK
ms.date: 03-19-2026
PlatyPS schema version: 2024-05-01
---

# Export-YubiKeyPIVCertificate

## SYNOPSIS

Export certificate from YubiKey PIV

## SYNTAX

### Slot

```
Export-YubiKeyPIVCertificate -Slot <PIVSlot> [-OutFile <FileInfo>] [-PEMEncoded]
 [<CommonParameters>]
```

### AttestationCertificate

```
Export-YubiKeyPIVCertificate -AttestationIntermediateCertificate [-OutFile <FileInfo>] [-PEMEncoded]
 [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Export certificates from YubiKey

## EXAMPLES

### Example 1

```powershell
PS C:\> $Certificate = Export-YubikeyPIVCertificate -Slot 0x9a
```

Exports the certificate to a variable for futher processing.

### Example 2

```powershell
PS C:\> Export-YubikeyPIVCertificate -Slot 0x9a -OutFile "$($env:TEMP)\exported_certificate.cer"
```

Exports the certificate from slot 0x9a and stores it as exported_certificate.cer in the temp folder.

### Example 3

```powershell
PS C:\> Export-YubikeyPIVCertificate -AttestationCertificate -OutFile yubikey_intermediate_attestation.cer
```

Exports the builtin intermediate attestation certificate.

## PARAMETERS

### -AttestationIntermediateCertificate

Export Attestation certificate

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases:
- AttestationCertificate
ParameterSets:
- Name: AttestationCertificate
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -OutFile

Output file

```yaml
Type: System.IO.FileInfo
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PEMEncoded

Encode output as PEM

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Slot

Slot to extract

```yaml
Type: powershellYK.PIV.PIVSlot
DefaultValue: None
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Slot
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

