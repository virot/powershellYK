# Enroll YubiKey FIDO2 against demo.yubico.com

### Lets start by creating the information prior to creation.
```pwsh
$username = "powershellYK$($(new-guid).tostring().Replace('-',''))"
$password = (get-date -Format 'yyyy-MM-dd')
$site = "demo.yubico.com"
```

### Create the user in the Yubico playground
```pwsh
$createUser = @{
'displayName'='powershellYK Demo';
'namespace'='playground';
'username'=$username;
'password'=$password
} | ConvertTo-JSON
$userCreation = Invoke-RestMethod -Method Post -SessionVariable session -Uri "https://$site/api/v1/user" -Body $createUser -ContentType 'application/json'
```

### Lets begin registering the YubiKey
```pwsh
$registerBeginBody = @{'authenticatorAttachment' = 'cross-platform'; 'residentKey' = $true} | ConvertTo-JSON
$registerBeginReturn = Invoke-RestMethod -Method Post -WebSession $session -Uri "https://$site/api/v1/user/$($userCreation.data.uuid)/webauthn/register-begin" -Body $registerBeginBody -ContentType 'application/json'

$userEntity = [Yubico.YubiKey.Fido2.UserEntity]::new([system.convert]::FromBase64String($registerBeginReturn.data.publicKey.user.id.'$base64'))
$userEntity.Name = $registerBeginReturn.data.publicKey.user.name
$userentity.DisplayName = $registerBeginReturn.data.publicKey.user.displayname
$out = New-YubiKeyFIDO2Credential -RelyingPartyID $registerBeginReturn.data.publicKey.rp.id -RelyingPartyName $registerBeginReturn.data.publicKey.rp.name -Discoverable $true  -Challenge $registerBeginReturn.data.publicKey.challenge.'$base64' -UserEntity $userEntity
```

### Return the attestion data etc to the site
This Data is lost by the SDK so we need to build it backup. Wonder if this is where it breaks.
```pwsh
$registerFinishBody = @{
    'requestId' = $registerBeginReturn.data.requestId;
    'attestation' = @{
        'attestationObject' = @{'$base64'=$out.GetBase64AttestationObject()};
        'clientDataJSON' = @{'$base64'=$out.GetBase64clientDataJSON()}
    }
} | ConvertTo-JSON -Compress
$registerFinishReturn = Invoke-RestMethod -Method Post -WebSession $session -Uri "https://$site/api/v1/user/$($userCreation.data.uuid)/webauthn/register-finish" -Body $registerFinishBody -ContentType 'application/json'
```

Now you can surf into [Yubikey Demo Site](https://demo.yubico.com/) and logon with your onboarded YubiKey.

