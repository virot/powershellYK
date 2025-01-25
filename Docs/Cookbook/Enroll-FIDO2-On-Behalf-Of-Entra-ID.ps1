<#
    .SYNOPSIS
        Enrolls and registers a YubiKey as device-bound passkey (FIDO2) for a user in Microsoft Entra ID (formerly Azure AD)

    .DESCRIPTION
        This script automates the enrollment of a YubiKey as device-bound passkey (FIDO2) credential in Microsoft Entra ID 
        using PowerShellYK and Microsoft Graph API. It supports both immediate and delayed attestation workflows:

        - Immediate attestation: Complete the enrollment in a single step
        - Delayed attestation: Split the process into credential creation and attestation phases, 
          allowing secure transport of unattested YubiKeys

        The only physical interaction required is touching the YubiKey when prompted during credential creation.
        Administrator rights (on Windows) and proper Microsoft Graph API permissions are required.

    .PARAMETER UserID
        The User Principal Name (UPN) or Object ID of the target user in Entra ID

    .PARAMETER DisplayName
        Optional. A friendly name (nickname) for the YubiKey in Entra ID (3-30 characters)
        Default: Combines YubiKey model name and serial number

    .PARAMETER ChallengeTimeoutInMinutes
        Optional. The time window allowed to complete the enrollment process
        Default: 5 minutes

    .PARAMETER AttestionFileOut
        Optional. Path to save the attestation data for delayed enrollment
        Used with delayed attestation workflow to save credential data for later use

    .PARAMETER AttestionFileIn
        Optional. Path to previously saved attestation data file
        Used to complete delayed attestation of a previously created credential

    .EXAMPLE
        Enroll-FIDO2-On-Behalf-Of-Endra-ID.ps1 -UserID bob@contoso.com
        Performs complete enrollment for the specified user in a single step

    .EXAMPLE
        Enroll-FIDO2-On-Behalf-Of-Endra-ID.ps1 -UserID bob@contoso.com -DisplayName "Backup YubiKey"
        Enrolls the YubiKey with a custom display name in Entra ID

    .EXAMPLE
        Enroll-FIDO2-On-Behalf-Of-Endra-ID.ps1 -UserID bob@contoso.com -AttestionFileOut "C:\temp\key1.xml"
        Creates a FIDO2 credential and saves the attestation data without registering it in Entra ID

    .EXAMPLE
        Enroll-FIDO2-On-Behalf-Of-Endra-ID.ps1 -UserID bob@contoso.com -AttestionFileIn "C:\temp\key1.xml"
        Completes the registration using previously saved attestation data

    .NOTES
        Requires:
        - PowerShell Core
        - PowerShellYK module
        - Microsoft.Graph.Authentication module
        - Administrator rights
        - Microsoft Graph API scope: UserAuthenticationMethod.ReadWrite.All

    .LINK
        https://github.com/virot/powershellYK/docs/Cookbook/Enroll-FIDO2-On-Behalf-Of-Endra-ID.ps1

    .LINK
        https://github.com/virot/powershellYK
#>

#Requires -PSEdition Core
#Requires -Modules @{ModuleName='powershellYK'; ModuleVersion='0.0.21.0'}, Microsoft.Graph.Authentication
#Requires -RunAsAdministrator
[CmdletBinding(DefaultParameterSetName = 'Complete')]
param
(
    [Parameter(Mandatory=$True,
              HelpMessage = "The User Principal Name (UPN) or Object ID of the target user")]
    [ValidateNotNullOrEmpty()]
    [string]
    $UserID,

    [Parameter(Mandatory=$False,
              HelpMessage = "Name of the YubiKey in Entra ID (3-30 characters)")]
    [ValidateNotNullOrEmpty()]
    [ValidateLength(3,30)]
    [string]
    $DisplayName = "$((Get-YubiKey).PrettyName) $((Get-YubiKey).SerialNumber)",

    [Parameter(Mandatory=$False,
              ParameterSetName="CreateCredential",
              HelpMessage = "Challenge timeout in minutes (1-60)")]
    [ValidateRange(1,60)]
    [int]
    $ChallengeTimeoutInMinutes = 5,

    [Parameter(Mandatory=$False,
              ParameterSetName="CreateCredential",
              HelpMessage = "Path to save attestation data")]
    [ValidateNotNullOrEmpty()]
    [string]
    $AttestionFileOut,

    [Parameter(Mandatory=$False,
              ParameterSetName="AttestCredential",
              HelpMessage = "Path to attestation data file")]
    [ValidateScript({
        if (-not (Test-Path -Path $_ -PathType Leaf)) {
            throw "Attestation file '$_' does not exist."
        }
        return $true
    })]
    [string]
    $AttestionFileIn
)
Begin
{
    # Set strict error handling
    $ErrorActionPreference = 'Stop'
    
    # Start logging
    $logFile = Join-Path $env:TEMP "FIDO2-Enrollment-$(Get-Date -Format 'yyyyMMdd-HHmmss').log"
    Start-Transcript -Path $logFile

    try {
        # Notify the user about authentication requirements
        Clear-Host
        Write-Host "NOTE: You may need to authenticate in the browser now (press any key to continue)"
        [System.Console]::ReadKey() > $null
        Clear-Host

        # Connect to Microsoft Graph API with required scopes
        Connect-MgGraph -Scopes 'UserAuthenticationMethod.ReadWrite.All' -NoWelcome

        # Verify Microsoft Graph authentication and permissions
        if ($null -eq $(Get-MgContext)) {
            Throw "Authentication and/or consent needed! Make sure you approve requested scopes!"
        }
        
        if (-not (Microsoft.Graph.Authentication\Get-MgContext).Scopes.Contains('UserAuthenticationMethod.ReadWrite.All')) {
            throw "Missing required Microsoft Graph permission: UserAuthenticationMethod.ReadWrite.All"
        }

        # YubiKey validation
        $yubiKey = Get-YubiKey
        if (-not $yubiKey) {
            throw "No YubiKey detected. Please insert a YubiKey and try again."
        }

        $fido2Status = Get-YubiKeyFIDO2
        if (-not $fido2Status.Options['clientPin']) {
            throw "YubiKey FIDO2 PIN is not set. Please set a PIN before continuing."
        }

        Write-Debug "YubiKey detected: $($yubiKey.PrettyName) (Serial: $($yubiKey.SerialNumber))"
    }
    catch {
        Stop-Transcript
        throw $_
    }
}
Process
{
    Write-Debug -Message "Running with ParameterSetName=$($PSCmdlet.ParameterSetName)"
    
    # Validate UserID format
    if ($UserID -notmatch '^[^@]+@[^@]+\.[^@]+$' -and $UserID -notmatch '^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$') {
        throw "Invalid UserID format. Please provide either a valid UPN (email format) or Object ID (GUID format)"
    }

    if ($PSCmdlet.ParameterSetName -in @('CreateCredential','Complete'))
    {
        Write-Debug -Message "Starting passkey (FIDO2) credential creation in Entra ID"
        try {
            $FIDO2Options = Invoke-MgGraphRequest -Method "GET" `
                -Uri "/beta/users/$UserID/authentication/fido2Methods/creationOptions(challengeTimeoutInMinutes=$ChallengeTimeoutInMinutes)" `
                -ErrorAction Stop
        }
        catch {
            throw "Failed to get passkey (FIDO2) creation options. Error: $($_.Exception.Message)"
        }

        # Prepare the response to send to the YubiKey.
        $challenge = [powershellYK.FIDO2.Challenge]::new($FIDO2Options.publicKey.challenge)
        $userEntity = [Yubico.YubiKey.Fido2.UserEntity]::new([System.Convert]::FromBase64String("$($FIDO2Options.publicKey.user.id -replace "-","+" -replace "_","/")"))
        $userEntity.Name = $FIDO2Options.publicKey.user.name
        $userentity.DisplayName = $FIDO2Options.publicKey.user.displayName
        $RelyingParty = [Yubico.YubiKey.Fido2.RelyingParty]::new($FIDO2Options.publicKey.rp.id)
        $Algorithms = $FIDO2Options.publicKey.pubKeyCredParams|Select -Exp Alg

        # Inform user to touch the YubiKey when it starts blinking...
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
            $result = Invoke-MgGraphRequest -Method "POST" `
                                          -Uri "https://graph.microsoft.com/beta/users/$UserID/authentication/fido2Methods" `
                                          -OutputType "Json" `
                                          -ContentType 'application/json' `
                                          -Body ($ReturnJSON | ConvertTo-JSON -Depth 4)
            
            Write-Host -ForegroundColor Green -Message "YubiKey successfully onboarded with displayname: $($ReturnJSON.displayName)"
        }
        catch
        {
            $errorMessage = if ($_.ErrorDetails) { $_.ErrorDetails } else { $_.Exception.Message }
            Write-Error -Message "Failed to onboard YubiKey. Attestation step failed: $errorMessage"
        }
    }
}
End
{
    # Stop logging
    Stop-Transcript

    # Cleanup
    if ($PSCmdlet.ParameterSetName -eq 'Complete') {
        Write-Host "Enrollment completed successfully. Log file: $logFile"
    }
    elseif ($PSCmdlet.ParameterSetName -eq 'CreateCredential') {
        Write-Host "Credential creation completed. Attestation data saved to: $AttestionFileOut"
    }
    elseif ($PSCmdlet.ParameterSetName -eq 'AttestCredential') {
        Write-Host "Attestation completed successfully"
    }
}
