---
external help file: powershellYK.dll-Help.xml
Module Name: powershellYK
online version:
schema: 2.0.0
---

# ConvertTo-AltSecurity

## SYNOPSIS
Generate the alt security security identities for a certificate

## SYNTAX

### From Certificate (Default)
```
ConvertTo-AltSecurity [-Certificate] <PSObject> [<CommonParameters>]
```

### From CertificateRequest
```
ConvertTo-AltSecurity -CertificateRequest <PSObject> [<CommonParameters>]
```

## DESCRIPTION
Creates all altSecurityIdentities and ssh keys for a given certificate. The altSecurityIdentities are created by taking the certificate's subject and issuer and creating a UPN and SPN from them. The ssh keys are created by taking the certificate's public key and creating the public part of a ssh key pair from it.

## EXAMPLES

### Example 1
```powershell
PS C:\>  ConvertTo-AltSecurity -Certificate SignedCertificate.cer

sshAuthorizedkey       : ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAAAgQDom6Eb5e6vpglN/YUAFAETRt0rlg7SBZZrJtZL8hiGR5z4tTwH8HRFQG
                         9MiT8kjqPwDNJMaR7mXrvrthLGs6PBHyWm1d6VKSjiiVICTglXr/KkEgByHaEqdo4WpB+Qs0GEhKjr1Ly2l9U2DpBCQUkD
                         0Gl/vefvZVqT8PpbdglA6Q== CN=SubjectName
X509IssuerSubject      : X509:<I>CN=test-WIN-UNORS4P71FA-CA, DC=test, DC=virot, DC=eu<S>CN=SubjectName
X509SubjectOnly        : X509:<S>CN=SubjectName
X509RFC822             :
X509IssuerSerialNumber : X509:<I>CN=test-WIN-UNORS4P71FA-CA, DC=test, DC=virot,
                         DC=eu<SR>4D0000079348418CEBD4B0991C000000000793
X509SKI                : X509:<SKI>E53887FCE0909BB0FF283E5255846B0DD5D86591
X509SHA1PublicKey      : X509:<SHA1-PUKEY>DE8C524F6EB39431DD4CDAAEC965BE7143CCCD79
```

Create all versions of altSecurityIdentities and ssh keys for the certificate SignedCertificate.cer

## PARAMETERS

### -Certificate
Certificate to extract info from

```yaml
Type: PSObject
Parameter Sets: From Certificate
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -CertificateRequest
Certificate request

```yaml
Type: PSObject
Parameter Sets: From CertificateRequest
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

### System.Management.Automation.PSObject

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
