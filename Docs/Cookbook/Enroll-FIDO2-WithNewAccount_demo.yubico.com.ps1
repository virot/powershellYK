#Requires -PSEdition Core
#Requires -RunAsAdministrator
param
(
    [Parameter(Mandatory=$False,
              HelpMessage = "Name of the user on demo.yubico.com")]
    [ValidateNotNullOrEmpty()]
    [string]
    $Username = "powershellYK$($(new-guid).tostring().Replace('-',''))",
    [Parameter(Mandatory=$False,
              HelpMessage = "Password for the user")]
    [ValidateNotNullOrEmpty()]
    [string]
    $Password = (get-date -Format 'yyyy-MM-dd')
)
Begin
{

    if ((Get-YubiKey -ErrorAction SilentlyContinue) -eq $Null)
    {
        throw "No YubiKey connected, connect YubiKey with Connect-YubiKey"
    }
    # Use the Begin to create the user, then the Process to add the FIDO2 credential

$createUser = @{
'displayName'='powershellYK Demo';
'namespace'='playground';
'username'=$username;
'password'=$password
} | ConvertTo-JSON
$userCreation = Invoke-RestMethod -Method Post -SessionVariable session -Uri "https://demo.yubico.com/api/v1/user" -Body $createUser -ContentType 'application/json'

}

Process
{
$registerBeginBody = @{'authenticatorAttachment' = 'cross-platform'; 'residentKey' = $true} | ConvertTo-JSON
$registerBeginReturn = Invoke-RestMethod -Method Post -WebSession $session -Uri "https://demo.yubico.com/api/v1/user/$($userCreation.data.uuid)/webauthn/register-begin" -Body $registerBeginBody -ContentType 'application/json'

$userEntity = [Yubico.YubiKey.Fido2.UserEntity]::new([system.convert]::FromBase64String($registerBeginReturn.data.publicKey.user.id.'$base64'))
$userEntity.Name = $registerBeginReturn.data.publicKey.user.name
$userentity.DisplayName = $registerBeginReturn.data.publicKey.user.displayname
$out = New-YubiKeyFIDO2Credential -RelyingPartyID $registerBeginReturn.data.publicKey.rp.id -RelyingPartyName $registerBeginReturn.data.publicKey.rp.name -Discoverable $true  -Challenge $registerBeginReturn.data.publicKey.challenge.'$base64' -UserEntity $userEntity

$registerFinishBody = @{
    'requestId' = $registerBeginReturn.data.requestId;
    'attestation' = @{
        'attestationObject' = @{'$base64'=$out.GetBase64AttestationObject()};
        'clientDataJSON' = @{'$base64'=$out.GetBase64clientDataJSON()}
    }
} | ConvertTo-JSON -Compress
$registerFinishReturn = Invoke-RestMethod -Method Post -WebSession $session -Uri "https://demo.yubico.com/api/v1/user/$($userCreation.data.uuid)/webauthn/register-finish" -Body $registerFinishBody -ContentType 'application/json'

}