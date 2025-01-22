# UNDER CONSTRUCTION
# BROKEN

### Install prerequisite
```pwsh
Install-ModuLe PowershellYK
Install-ModuLe Microsoft.Graph.Authentication
Install-Module DSInternals.Passkeys

```

### Lets start by creating the information prior to creation.
```pwsh
$userid = 'test.yubikey@toriv.com'
$tenent = 'toriv.onmicrosoft.com'
```

### Install things that we need
```pwsh
Import-Module powershellYK
Import-ModuLe Microsoft.Graph.Authentication
Import-Module DSInternals.Passkeys
```

### Lets connect to Microsoft Graph
```pwsh
Connect-MgGraph -Scopes UserAuthenticationMethod.ReadWrite.All -TenantId $tenant -NoWelcome
$RegistrationOptions = $RegistrationOptions = Get-PasskeyRegistrationOptions -UserId $userid
```

### Lets begin registering the YubiKey
```pwsh
$challenge = [powershellYK.FIDO2.Challenge]::new($RegistrationOptions.PublicKeyOptions.Challenge)
$userEntity = [Yubico.YubiKey.Fido2.UserEntity]::new($RegistrationOptions.PublicKeyOptions.User.Id)
$userEntity.Name = $RegistrationOptions.PublicKeyOptions.User.Name
$userentity.DisplayName = $RegistrationOptions.PublicKeyOptions.User.DisplayName
$RelyingParty = [Yubico.YubiKey.Fido2.RelyingParty]::new($RegistrationOptions.PublicKeyOptions.RelyingParty.id)

$passkey = New-YubiKeyFIDO2Credential -RelyingParty $RelyingParty -Discoverable $true  -challenge $challenge -UserEntity $userEntity
```

### Return the attestion data etc to the site
This Data is lost by the SDK so we need to build it backup. Wonder if this is where it breaks.
```pwsh
$a = [powershellYK.FIDO2.CredentialData]::new($passkey)
$clientDataJSON = @{
    'type' = 'webauthn.create';
    'challenge' = $challenge.UrlEncode();
    'origin' = "https://login.microsoft.com";
    'crossOrigin' = $false
} | ConvertTo-JSON -Compress

$return = @{
  'displayName' = 'Name of yubikey';
  'publicKeyCredential' = @{
    'id' = [convert]::ToBase64String($passkey.AuthenticatorData.CredentialId.id.ToArray()) -replace '\+', '-' -replace '/', '_' -replace '=','';
    'response' = @{
      'clientDataJSON' = [system.convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes($clientDataJSON));
      'attestationObject' = [system.convert]::ToBase64String($a.w3cEncoded())
    }     
  }
}| ConvertTo-JSON -Depth 4

$URI = "https://graph.microsoft.com/beta/users/$userid/authentication/fido2Methods"
$response = Invoke-MgGraphRequest -Method "POST" -Uri $URI -OutputType "Json" -ContentType 'application/json' -Body $return
```

Here we should be done, but something is broken somewhere..

