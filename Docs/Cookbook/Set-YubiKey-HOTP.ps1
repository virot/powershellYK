<#
.SYNOPSIS
Programs HOTP to a slot on a YubiKey and outputs to seed file.

.DESCRIPTION
This Cmdlet programs HOTP (HMAC-based One-Time Password) to a selected slot on a YubiKey.
It can configure either the short press slot (1) or long press slot (2).
The configuration includes options for sending TAB before the code and appending a carriage return.

.PARAMETER ShortPress
Programs (H)OTP to YubiKey slot 1

.PARAMETER LongPress
Programs (H)OTP to YubiKey slot 2

.PARAMETER SendTabFirst
Sends TAB before the passcode to navigate UI

.PARAMETER AppendCarriageReturn
Appends a carriage return (Enter) after the passcode

.PARAMETER Use8Digits
Use 8 digits instead of 6 for the passcode

.EXAMPLE
.\Set-YubiKey-HOTP.ps1 -ShortPress
Programs HOTP to slot 1 (short press) on the YubiKey.

.EXAMPLE
.\Set-YubiKey-HOTP.ps1 -LongPress -SendTabFirst -AppendCarriageReturn
Programs HOTP to slot 2 (long press) with TAB before the code and Enter after.

.EXAMPLE
.\Set-YubiKey-HOTP.ps1 -ShortPress -Use8Digits
Programs HOTP to slot 1 with 8-digit passcodes.

.NOTES
- Requires the powerShellYK module
- Creates/updates a CSV file named 'hotp-secrets.csv' in the current directory
- Each configuration generates a new secret key

.LINK
https://github.com/virot/powershellYK/
#>

# Function with parameters
function Set-YubiKeyHOTP {
    [CmdletBinding(DefaultParameterSetName = 'ShortPress')]
    param
    (
        [Parameter(Mandatory=$True,
                  ParameterSetName = 'ShortPress',
                  HelpMessage = "Programs (H)OTP to YubiKey slot 1")]
        [switch]
        $ShortPress,

        [Parameter(Mandatory=$True,
                  ParameterSetName = 'LongPress',
                  HelpMessage = "Programs (H)OTP to YubiKey slot 2")]
        [switch]
        $LongPress,

        [Parameter(Mandatory=$False,
                  HelpMessage = "Sends TAB before the passcode to navigate UI")]
        [switch]
        $SendTabFirst,

        [Parameter(Mandatory=$False,
                  HelpMessage = "Appends a carriage return (Enter) after the passcode")]
        [switch]
        $AppendCarriageReturn,

        [Parameter(Mandatory=$False,
                  HelpMessage = "Use 8 digits instead of 6 for the passcode")]
        [switch]
        $Use8Digits
    )

    begin {
        # Check if a CSV output file exists else create it
        $csvFilePath = Join-Path -Path (Get-Location) -ChildPath "hotp-secrets.csv"
        if (-Not (Test-Path -Path $csvFilePath)) {
            Write-Debug "No existing CSV file found in the current directory. Creating a new one..."
            New-Item -Path $csvFilePath -ItemType File -Force | Out-Null
            Set-Content -Path $csvFilePath -Value "Serial,Secret,Counter,Length" -Encoding UTF8
        } else {
            Write-Debug "Found existing CSV file in the current directory. Appending to it..."
        }
        # Suppress informational messages
        $InformationPreference = 'SilentlyContinue'

        # Warn the user on pending configuration:
        Clear-Host
        Write-Warning "Please insert a YubiKey NOW and press any key to continue:"
        [System.Console]::ReadKey() > $null
        Clear-Host
        
        # Connect to YubiKey
        Connect-Yubikey

        # Get the YubiKey device
        $yubiKey = Get-YubiKey
        if ($null -eq $yubiKey) {
            throw "Failed to get YubiKey device"
        }

        # Determine which slot to configure
        $slot = if ($ShortPress) { [Yubico.YubiKey.Otp.Slot]::ShortPress } else { [Yubico.YubiKey.Otp.Slot]::LongPress }

        Write-Host "Configuring HOTP..." -ForegroundColor Yellow
        $result = Set-YubiKeyOTP -Slot $slot -HOTP -SendTabFirst:$SendTabFirst -AppendCarriageReturn:$AppendCarriageReturn

        # Save the configuration to CSV
        [PSCustomObject]@{
            'Serial' = $yubiKey.SerialNumber
            'Secret' = $result.Base32Secret
            'Counter' = 0
            'Length' = if ($Use8Digits) { 8 } else { 6 }
        } | Export-Csv -Path $csvFilePath -Append -NoTypeInformation -UseQuotes Never

        # Disconnect from YubiKey
        Disconnect-YubiKey

        # Display summary
        #Clear-Host
        Write-Host "*******************************************" -ForegroundColor Yellow
        Write-Host "HOTP SUCCESSFULLY CONFIGURED ON YUBIKEY!" -ForegroundColor Yellow
        Write-Host "*******************************************" -ForegroundColor Yellow
        Write-Host "📝 Information saved to: $csvFilePath" -ForegroundColor Yellow
        Write-Host ""
    }
}

# Call the function with the provided parameters
Set-YubiKeyHOTP @args
