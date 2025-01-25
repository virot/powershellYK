<#
    .SYNOPSIS
        Enroll a YubiKey FIDO2 credential for a User
    .DESCRIPTION
        This scripts uses powershellYK and MS Graph to onboard a user with a YubiKey without the need to interaction other than pushing the touch.
        This script also allows for delayed Attestion, where the YubiKey can be transported without being attested in EntraId, making it unusable.
    .PARAMETER UserID
        The ID or UPN of the user that we are enrolling for
    .EXAMPLE
        Enroll-FIDO2-On-Behalf-Of-Endra-ID.ps1 -UserID test.yubikey@virot.eu

        Do a complete enrollment for the user specified
    .EXAMPLE
        Enroll-FIDO2-On-Behalf-Of-Endra-ID.ps1 -UserID test.yubikey@virot.eu -DisplayName "Dont loose this one"

        Do a complete enrollment for the user specified and name the FIDO2 credential in Entra ID
    .EXAMPLE
        Enroll-FIDO2-On-Behalf-Of-Endra-ID.ps1 -UserID test.yubikey@virot.eu -AttestionFileOut usersave.xml

        Create a fido2 credential but do not attest it, rather save it for a later attest.
    .EXAMPLE
        Enroll-FIDO2-On-Behalf-Of-Endra-ID.ps1 -UserID test.yubikey@virot.eu -AttestionFileIn usersave.xml

        Attest the earler created credential. This will make it functional.

    .LINK
        https://github.com/virot/powershellYK/docs/Cookbook/Enroll-FIDO2-On-Behalf-Of-Endra-ID.ps1

    .LINK
        https://github.com/virot/powershellYK
#>

#Requires -PSEdition Core
#Requires -Modules powershellYK, Microsoft.Graph.Authentication
#Requires -RunAsAdministrator
[CmdletBinding(DefaultParameterSetName = 'Complete')]
param
(
    [Parameter(Mandatory=$True,HelpMessage = "The ID or UPN of the user that we are enrolling the ")]
    [string]
    $UserID,
    [Parameter(Mandatory=$False,HelpMessage = "Name of the FIDO2 Key in Entra ID")]
    [string]
    [ValidateLength(3,30)]
    $DisplayName = "$((Get-YubiKey).PrettyName) $((Get-YubiKey).SerialNumber)",
    [Parameter(Mandatory=$False,HelpMessage = "The amount of minutes the Challange will be valid for, this is the time to complete the enrollment.", ParameterSetName="CreateCredential")]
    [int]
    $ChallengeTimeoutInMinutes=5,
    [Parameter(Mandatory=$False,HelpMessage = "File to write the attestion data into", ParameterSetName="CreateCredential")]
    [string]
    $AttestionFileOut,
    [ValidateScript({if (-not (Test-Path -Path $_ -PathType Leaf)){throw "File $_ does not exist."}else{$true}})]
    [Parameter(Mandatory=$False,HelpMessage = "What action should be completed", ParameterSetName="AttestCredential")]
    [string]
    $AttestionFileIn
)
Begin
{
  # 
  if (
    (Get-MgContext) -eq $Null -or  # Verify that we are authenticated to MSGraph
    (-not (Microsoft.Graph.Authentication\Get-MgContext).Scopes.Contains('UserAuthenticationMethod.ReadWrite.All')) # Verify that we have the require permissions
  )
  {
    throw "Not connected with correct permissions to MSGraph, run 'Connect-MgGraph -Scopes UserAuthenticationMethod.ReadWrite.All'"
  }
  if ((Microsoft.Graph.Authentication\Get-MgContext).Account -eq $UserID) # This is checking for same user as authenticated to Graph
  {
    throw "Microsoft does not allow Enrollment On Behalf Of same user."
  }
  if (
    (Get-YubiKey) -eq $Null -or  # No YubiKey inserted
    (Get-YubiKeyFIDO2).Options['clientPin'] -eq $False  # No PIN set
  )
  {
    throw "Make sure to insert a YubiKey with a set FIDO2 PIN"
  }
}
Process
{
  Write-Debug -Message "Running with ParameterSetName=$($PSCmdlet.ParameterSetName)"
  if ($PSCmdlet.ParameterSetName -in @('CreateCredential','Complete'))
  {
    Write-Debug -Message "Starting to create parameter in Entra ID"
    $FIDO2Options = Invoke-MgGraphRequest -Method "GET" -Uri "/beta/users/$userid/authentication/fido2Methods/creationOptions(challengeTimeoutInMinutes=$($challengeTimeoutInMinutes ?? "5"))"

    # prepair the resonse to send to the YubiKey.
    $challenge = [powershellYK.FIDO2.Challenge]::new($FIDO2Options.publicKey.challenge)
    $userEntity = [Yubico.YubiKey.Fido2.UserEntity]::new([System.Convert]::FromBase64String("$($FIDO2Options.publicKey.user.id -replace "-","+" -replace "_","/")"))
    $userEntity.Name = $FIDO2Options.publicKey.user.name
    $userentity.DisplayName = $FIDO2Options.publicKey.user.displayName
    $RelyingParty = [Yubico.YubiKey.Fido2.RelyingParty]::new($FIDO2Options.publicKey.rp.id)
    $Algorithms = $FIDO2Options.publicKey.pubKeyCredParams|Select -Exp Alg

    # Inform user to press the YubiKey touch..
    Write-Host -Message "Please touch the YubiKey when it starts blinking..." -ForegroundColor Yellow

    # The actual creation of the credential
    $FIDO2Response = New-YubiKeyFIDO2Credential -RelyingParty $RelyingParty -Discoverable $true -Challenge $challenge -UserEntity $userEntity -RequestedAlgorithms $Algorithms
    $ReturnJSON = @{
      'displayName' = $DisplayName.Trim();
      'publicKeyCredential' = @{
        'id' = $FIDO2Response.GetBase64UrlSafeCredentialID();
        'response' = @{
          'clientDataJSON' = $FIDO2Response.GetBase64clientDataJSON();
          'attestationObject' = $FIDO2Response.GetBase64AttestationObject()
        }     
      }
    }
  }
  if ($PSCmdlet.ParameterSetName -eq 'CreateCredential')
  {
    Export-Clixml -Path $AttestionFileOut -InputObject $ReturnJSON
  }
  if ($PSCmdlet.ParameterSetName -eq 'AttestCredential')
  {
    $ReturnJSON = Import-Clixml -Path $AttestionFileIn
  }
  if ($PSCmdlet.ParameterSetName -in @('AttestCredential','Complete'))
  {
  # End of $return
    if ($PSBoundParameters.ContainsKey('DisplayName'))
    {
      $ReturnJSON.displayName = $DisplayName.Trim();
    }
    try
    {
      Invoke-MgGraphRequest -Method "POST" -Uri "https://graph.microsoft.com/beta/users/$userid/authentication/fido2Methods" -OutputType "Json" -ContentType 'application/json' -Body ($ReturnJSON| ConvertTo-JSON -Depth 4) | ConvertFrom-Json
      Write-Host -ForegroundColor Green -Message "YubiKey onboarded with diplayname: $($DisplayName.Trim())"
    }
    Catch
    {
      Write-Error -Message "YubiKey failed to onboard. Attestion step failed with message $($_.ErrorDetails)"
    }
  }
}