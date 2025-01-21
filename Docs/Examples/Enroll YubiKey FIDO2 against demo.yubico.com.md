## UNDER CONSTRUCTION ##
# Lets start by creating the information prior to creation.
$username = "powershellYK$($(new-guid).tostring().Replace('-',''))"
$password = (get-date -Format 'yyyy-MM-dd')
$site = "demo.yubico.com"

# Now that we have a user and password, lets create the user in the Yubico playground
$createUser = @{
'displayName'='powershellYK Demo';
'namespace'='playground';
'username'=$username;
'password'=$password
} | ConvertTo-JSON

$userCreation = Invoke-RestMethod -Method Post -SessionVariable session -Uri "https://$site/api/v1/user" -Body $createUser -ContentType 'application/json'

#Lets begin registering the YubiKey
$registerBeginBody = @{'authenticatorAttachment' = 'cross-platform'; 'residentKey' = $true} | ConvertTo-JSON
$registerBeginReturn = Invoke-RestMethod -Method Post -WebSession $session -Uri "https://$site/api/v1/user/$($userCreation.data.uuid)/webauthn/register-begin" -Body $registerBeginBody -ContentType 'application/json'

$userEntity = [Yubico.YubiKey.Fido2.UserEntity]::new([system.convert]::FromBase64String($registerBeginReturn.data.publicKey.user.id.'$base64'))
$userEntity.Name = $registerBeginReturn.data.publicKey.user.name
$userentity.DisplayName = $registerBeginReturn.data.publicKey.user.displayname


$out = New-YubiKeyFIDO2Credential -RelyingPartyID $registerBeginReturn.data.publicKey.rp.id -RelyingPartyName $registerBeginReturn.data.publicKey.rp.name -Discoverable $true  -Challange $registerBeginReturn.data.publicKey.challenge.'$base64' -UserEntity $userEntity

# This Data is lost by the SDK so we need to build it backup. Wonder if this is where it breaks.
$a = [powershellYK.FIDO2.CredentialData]::new($out)
#[system.convert]::ToBase64String($a.w3cEncoded())

$clientDataJSON = @{
    'type' = 'webauthn.create';
    'challenge' = $registerBeginReturn.data.publicKey.challenge.'$base64' -replace '\+', '-' -replace '/', '_' -replace '=','';
    'origin' = "https://$site";
    'crossOrigin' = $false
} | ConvertTo-JSON -Compress

# Lets send stuff back to demo.yubico.com to enable the security key
$registerFinishBody = @{
    'requestId' = $registerBeginReturn.data.requestId;
    'attestation' = @{
        'attestationObject' = @{'$base64'=[system.convert]::ToBase64String($a.w3cEncoded())};
        'clientDataJSON' = @{'$base64'=[system.convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes($clientDataJSON))}
    }
} | ConvertTo-JSON -Compress
$registerFinishReturn = Invoke-RestMethod -Method Post -WebSession $session -Uri "https://$site/api/v1/user/$($userCreation.data.uuid)/webauthn/register-finish" -Body $registerFinishBody -ContentType 'application/json'
