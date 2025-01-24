# Enroll YubiKey FIDO2 against login.microsoft.com
To simplify I am using **DSInternals.Passkeys** as **Invoke-MgBetaCreationUserAuthenticationFido2MethodOption** is broken.

### Install prerequisite
```pwsh
Install-ModuLe PowershellYK
Install-ModuLe Microsoft.Graph.Authentication
```

### Lets start by creating the information prior to creation.
```pwsh
$userid = 'test.yubikey@virot.eu'
$tenent = '<your tenant>'
```

### Install things that we need
```pwsh
Import-Module powershellYK
Import-ModuLe Microsoft.Graph.Authentication
```

### Lets connect to Microsoft Graph
```pwsh
Connect-MgGraph -Scopes UserAuthenticationMethod.ReadWrite.All -TenantId $tenant -NoWelcome
```

### Lets get the Challenge data from Microsoft Azure
```pwsh
$FIDO2Options = Invoke-MgGraphRequest -Method "GET" -Uri "/beta/users/$userid/authentication/fido2Methods/creationOptions(challengeTimeoutInMinutes=$($challengeTimeoutInMinutes ?? "5"))" 
```

### Lets begin registering the YubiKey
Lets start with building the Userentity, Challenge and RelayingParty. This can be done or we can sned the different parts to the Cmdlets.
```pwsh
$challenge = [powershellYK.FIDO2.Challenge]::new($FIDO2Options.publicKey.challenge)
$userEntity = [Yubico.YubiKey.Fido2.UserEntity]::new([System.Convert]::FromBase64String("$($FIDO2Options.publicKey.user.id -replace "-","+" -replace "_","/")"))
$userEntity.Name = $FIDO2Options.publicKey.user.name
$userentity.DisplayName = $FIDO2Options.publicKey.user.displayName
$RelyingParty = [Yubico.YubiKey.Fido2.RelyingParty]::new($FIDO2Options.publicKey.rp.id)

$FIDO2Response = New-YubiKeyFIDO2Credential -RelyingParty $RelyingParty -Discoverable $true -Challenge $challenge -UserEntity $userEntity
```

### Return the data from the YubiKey to Microsoft so it will work
When we have enrolled the FIDO credential we need to send over some data to Azure
```pwsh
$return = @{
  'displayName' = 'Name of yubikey';
  'publicKeyCredential' = @{
    'id' = $FIDO2Response.GetBase64UrlSafeCredentialID();
    'response' = @{
      'clientDataJSON' = $FIDO2Response.GetBase64clientDataJSON();
      'attestationObject' = $FIDO2Response.GetBase64AttestationObject()
    }     
  }
}| ConvertTo-JSON -Depth 4

$URI = "https://graph.microsoft.com/beta/users/$userid/authentication/fido2Methods"
$response = Invoke-MgGraphRequest -Method "POST" -Uri $URI -OutputType "Json" -ContentType 'application/json' -Body $return
```

Your YubiKey is now onboarded.