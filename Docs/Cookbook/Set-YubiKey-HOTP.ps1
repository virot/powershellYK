<#
.SYNOPSIS
Programs HOTP to a slot on a YubiKey and outputs to seed file in Hex or Base32 format.

.DESCRIPTION
This Cmdlet programs HOTP (HMAC-based One-Time Password) to a selected slot on a YubiKey.
It can configure either the short press slot (1) or long press slot (2).
The configuration includes options for sending TAB before the code and appending a carriage return.
If a YubiKey with the same serial number already exists in the CSV file, its entry will be updated.

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

.PARAMETER SecretFormat
Secret format to use (Base32 or Hex)

.EXAMPLE
.\Set-YubiKey-HOTP.ps1 -ShortPress
Programs HOTP to slot 1 (short press) on the YubiKey using Base32 format (default).

.EXAMPLE
.\Set-YubiKey-HOTP.ps1 -LongPress -SendTabFirst -AppendCarriageReturn
Programs HOTP to slot 2 (long press) with TAB before the code and Enter after, using Base32 format.

.EXAMPLE
.\Set-YubiKey-HOTP.ps1 -ShortPress -Use8Digits
Programs HOTP to slot 1 with 8-digit passcodes using Base32 format.

.EXAMPLE
.\Set-YubiKey-HOTP.ps1 -ShortPress -SecretFormat Hex
Programs HOTP to slot 1 using hex format (recommended for Cisco Duo and Ping Identity).

.EXAMPLE
.\Set-YubiKey-HOTP.ps1 -LongPress -SecretFormat Hex -SendTabFirst -AppendCarriageReturn
Programs HOTP to slot 2 using hex format with TAB and Enter, suitable for Cisco Duo.

.NOTES
- Requires the powerShellYK module
- Creates/updates a CSV file named 'hotp-secrets.csv' in the current directory
- Each configuration generates a new secret key
- If a YubiKey with the same serial number is programmed again, its entry will be updated
- After programming, prompts to program another YubiKey (Y/n)
- For Cisco Duo and Ping Identity, use hex format with "-SecretFormat Hex"

.LINK
https://github.com/virot/powershellYK/
#>

# Function to program a single YubiKey
function Set-YubiKeyHOTPConfig {
    param (
        [Yubico.YubiKey.Otp.Slot]$Slot,
        [switch]$SendTabFirst,
        [switch]$AppendCarriageReturn,
        [switch]$Use8Digits,
        [string]$CsvFilePath,
        [ValidateSet('Base32', 'Hex')]
        [string]$SecretFormat = 'Base32'
    )

    # Connect to YubiKey
    try {
        Connect-Yubikey
        $yubiKey = Get-YubiKey
        if ($null -eq $yubiKey) {
            throw "No YubiKey found."
        }
    }
    catch {
        Clear-Host
        Write-Host "No YubiKey connected. Please try again." -ForegroundColor Red
        Write-Host ""
        return $false
    }

    Write-Host "Configuring HOTP..." -ForegroundColor Yellow
    $result = Set-YubiKeyOTP -Slot $slot -HOTP -SendTabFirst:$SendTabFirst -AppendCarriageReturn:$AppendCarriageReturn -Use8Digits:$Use8Digits

    # Create new configuration object
    $newConfig = [PSCustomObject]@{
        'Serial' = $yubiKey.SerialNumber
        'Secret' = if ($SecretFormat -eq 'Hex') { $result.HexSecret } else { $result.Base32Secret }
        'Counter' = 0
        'Length' = if ($Use8Digits) { 8 } else { 6 }
    }

    # Read existing CSV content
    $existingData = Import-Csv -Path $CsvFilePath
    $serialExists = $false

    # Check if serial exists and update if found
    $updatedData = @($existingData | ForEach-Object {
        if ($_.Serial -eq $yubiKey.SerialNumber) {
            $serialExists = $true
            $newConfig
        } else {
            $_
        }
    })

    # If serial didn't exist, append the new config
    if (-not $serialExists) {
        $updatedData = @($updatedData) + $newConfig
    }

    # Write back to CSV
    $updatedData | Export-Csv -Path $CsvFilePath -NoTypeInformation -UseQuotes Never

    # Disconnect from YubiKey
    Disconnect-YubiKey

    # Display user feedback
    Clear-Host
    Write-Host "*****************************************************************" -ForegroundColor Yellow
    Write-Host "YUBIKEY SUCCESSFULLY PROGRAMMED WITH HOTP TO SLOT!" -ForegroundColor Yellow
    Write-Host "*****************************************************************" -ForegroundColor Yellow
    if ($serialExists) {
        Write-Host "ℹ️ Updated existing entry for YubiKey with serial: $($yubiKey.SerialNumber)" -ForegroundColor Yellow
    } else {
        Write-Host "ℹ️ Added new entry for YubiKey with serial: $($yubiKey.SerialNumber)" -ForegroundColor Yellow
    }
    Write-Host "📝 Information saved to: $CsvFilePath" -ForegroundColor Yellow
    Write-Host ""

    return $true
}

# Main function with parameters
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
        $Use8Digits,

        [Parameter(Mandatory=$False,
                  HelpMessage = "Secret format to use (Base32 or Hex)")]
        [ValidateSet('Base32', 'Hex')]
        [string]
        $SecretFormat = 'Base32'
    )

    begin {
        # Check if a CSV output file exists else create it
        $csvFilePath = Join-Path -Path (Get-Location) -ChildPath "hotp-secrets.csv"
        if (-Not (Test-Path -Path $csvFilePath)) {
            Write-Debug "No existing CSV file found in the current directory. Creating a new one..."
            New-Item -Path $csvFilePath -ItemType File -Force | Out-Null
            Set-Content -Path $csvFilePath -Value "Serial,Secret,Counter,Length" -Encoding UTF8
        } else {
            Write-Debug "Found existing CSV file in the current directory. Will update if serial exists..."
        }
        # Suppress informational messages
        $InformationPreference = 'SilentlyContinue'

        # Determine which slot to configure
        $slot = if ($ShortPress) { [Yubico.YubiKey.Otp.Slot]::ShortPress } else { [Yubico.YubiKey.Otp.Slot]::LongPress }

        # Program first YubiKey
        Clear-Host
        Write-Warning "Please insert a YubiKey NOW and press any key to continue:"
        [System.Console]::ReadKey() > $null
        Clear-Host

        if (-not (Set-YubiKeyHOTPConfig -Slot $slot -SendTabFirst:$SendTabFirst -AppendCarriageReturn:$AppendCarriageReturn -Use8Digits:$Use8Digits -CsvFilePath $csvFilePath -SecretFormat $SecretFormat)) {
            return
        }

        # Ask if user wants to program another YubiKey
        do {
            do {
                $programAnother = Read-Host "Do you want to program another YubiKey? (Y/n)"
                $continue = $false
                
                switch ($programAnother.Trim().ToLower()) {
                    '' { $continue = $true }      # Empty input (enter) continues
                    'y' { $continue = $true }     # 'y' continues
                    'n' { $continue = $false }    # 'n' exits
                    default { 
                        Clear-Host
                        $continue = $null         # Invalid input, ask again
                    }
                }
            } while ($continue -eq $null)

            if ($continue) {
                Clear-Host
                Write-Warning "Please insert a YubiKey NOW and press any key to continue:"
                [System.Console]::ReadKey() > $null
                Clear-Host

                if (-not (Set-YubiKeyHOTPConfig -Slot $slot -SendTabFirst:$SendTabFirst -AppendCarriageReturn:$AppendCarriageReturn -Use8Digits:$Use8Digits -CsvFilePath $csvFilePath -SecretFormat $SecretFormat)) {
                    continue
                }
            }
        } while ($continue)
    }
}

# Call the function with the provided parameters
Set-YubiKeyHOTP @args
