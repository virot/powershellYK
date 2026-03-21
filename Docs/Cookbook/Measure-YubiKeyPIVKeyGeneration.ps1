<#
.SYNOPSIS
Benchmarks key generation performance (PIV) across all supported algorithms on a YubiKey.

.DESCRIPTION
Measures the time required to generate key pairs on a YubiKey for each supported
algorithm. Runs multiple iterations per algorithm and reports min, max, average timings.
Created keys are cleaned up after each iteration (pending support in firmware) or overwritten in place.
Results are returned as PSCustomObject arrays suitable for piping to Format-Table or ConvertTo-Json.

.PARAMETER Iterations
Number of timed key generation iterations per algorithm. Defaults to 5.

.PARAMETER Algorithm
Optional filter to benchmark only specific algorithms. If omitted, all algorithms
supported by the connected YubiKey are benchmarked. Valid values depend on the
YubiKey firmware (e.g., Rsa1024, Rsa2048, Rsa3072, Rsa4096, EcP256, EcP384,
Ed25519, X25519).

.PARAMETER Slot
PIV slot to use for key generation. Defaults to "Card Authentication" (0x9e).

.PARAMETER IncludeWarmup
Runs one untimed warmup iteration per algorithm before the timed iterations to
eliminate PIV applet initialization overhead.

.PARAMETER Force
Allows the benchmark to proceed when the target slot already contains a private key.
Without this switch, the script aborts to prevent accidental key destruction.

.EXAMPLE
.\Measure-YubiKeyPIVKeyGeneration.ps1
Benchmarks all supported algorithms with 5 iterations each.

.EXAMPLE
.\Measure-YubiKeyPIVKeyGeneration.ps1 -Iterations 10 -Algorithm rsa1024
Benchmarks only RSA 1024 with 10 iterations each.

.EXAMPLE
.\Measure-YubiKeyPIVKeyGeneration.ps1 -IncludeWarmup
Benchmarks all algorithms with a warmup pass.

.EXAMPLE
.\Measure-YubiKeyPIVKeyGeneration.ps1 -Slot "Digital Signature" -Force
Benchmarks key generation in the Digital Signature slot, overwriting any existing key.

.EXAMPLE
.\Measure-YubiKeyPIVKeyGeneration.ps1 -Slot 0x9c -Force
Same as above, using the hex slot identifier instead of the friendly name.

.NOTES
- Requires the powershellYK module
- Uses slot 9e (Card Authentication) with PIN and touch policy disabled
- Aborts if the target slot already contains a key unless -Force is specified
- Key cleanup after benchmarking requires firmware 5.7+; older firmware leaves the last key in the slot
- Large key sizes such as RSA 4096 can take several seconds per iteration; plan accordingly
- Timings include USB/host overhead and vary with OS, USB connection type, drivers, host load etc.
- Results reflect end-to-end command latency, not raw secure element performance.

.LINK
https://github.com/virot/powershellYK/
#>

#Requires -PSEdition Core
#Requires -Modules powershellYK

function Measure-YubiKeyPIVKeyGeneration {
    [CmdletBinding()]
    param(
        [Parameter(HelpMessage = "Number of timed iterations per algorithm")]
        [ValidateRange(1, 100)]
        [int]$Iterations = 5,

        [Parameter(HelpMessage = "Algorithm(s) to benchmark; defaults to all supported")]
        [ValidateSet("Rsa1024", "Rsa2048", "Rsa3072", "Rsa4096", "EcP256", "EcP384", "Ed25519", "X25519")]
        [string[]]$Algorithm,

        [Parameter(HelpMessage = "PIV slot to use for key generation")]
        [ArgumentCompletions("PIV Authentication", "Digital Signature", "Key Management", "Card Authentication", "0x9a", "0x9c", "0x9d", "0x9e")]
        [string]$Slot = "Card Authentication",

        [Parameter(HelpMessage = "Run one untimed warmup iteration per algorithm")]
        [switch]$IncludeWarmup,

        [Parameter(HelpMessage = "Overwrite any existing key in the target slot without prompting")]
        [switch]$Force
    )

    $results    = [System.Collections.Generic.List[PSCustomObject]]::new()
    $canDeleteKey = $false
    $connected  = $false
    $keyInSlot  = $false

    try {
        # Connect to a YubiKey
        Write-Host "Connecting to YubiKey..." -ForegroundColor Cyan
        try {
            Connect-YubiKey
            $connected = $true
        }
        catch {
            Write-Error "Failed to connect to YubiKey. Ensure a single YubiKey is inserted."
            return
        }

        $yubiKeyInfo = Get-YubiKey
        if ($null -eq $yubiKeyInfo) {
            Write-Error "No YubiKey found. Please insert a YubiKey and try again."
            return
        }

        # PIV Support Check
        $pivInfo = $null
        try {
            $pivInfo = Get-YubiKeyPIV
        }
        catch {
            Write-Error "Failed to access PIV application. This YubiKey may not support PIV."
            return
        }

        if ($null -eq $pivInfo) {
            Write-Error "PIV application not available on this YubiKey."
            return
        }

        # Determine what algorithms are supported
        $supportedAlgorithms = $pivInfo.SupportedAlgorithms

        if ($Algorithm) {
            $unsupported = $Algorithm | Where-Object { $_ -notin $supportedAlgorithms }
            if ($unsupported) {
                Write-Warning "The following algorithms are not supported by this YubiKey and will be skipped: $($unsupported -join ', ')"
            }
            $algorithmsToTest = @($Algorithm | Where-Object { $_ -in $supportedAlgorithms })
        }
        else {
            $algorithmsToTest = @($supportedAlgorithms)
        }

        if (-not $algorithmsToTest -or $algorithmsToTest.Count -eq 0) {
            Write-Error "No supported algorithms to benchmark."
            return
        }

        # Determine if created key(s) can be deleted following test
        $canDeleteKey = $yubiKeyInfo.FirmwareVersion -ge [Yubico.YubiKey.FirmwareVersion]::new(5, 7, 0)
        if (-not $canDeleteKey) {
            Write-Warning "Firmware does not support key deletion (a test key will remain in the slot)."
        }

        # Detect and warn the user if there is an existing key in the target slot
        $slotsWithKeys = $pivInfo.SlotsWithPrivateKeys
        $slotByte = switch ($Slot) {
            "PIV Authentication"  { 0x9a }
            "Digital Signature"   { 0x9c }
            "Key Management"      { 0x9d }
            "Card Authentication" { 0x9e }
            "0x9a"                { 0x9a }
            "0x9c"                { 0x9c }
            "0x9d"                { 0x9d }
            "0x9e"                { 0x9e }
            default {
                Write-Error "Unknown slot '$Slot'. Use a friendly name (e.g. 'Card Authentication') or hex value (e.g. '0x9e')."
                return
            }
        }

        $slotName = switch ($slotByte) {
            0x9a { "PIV Authentication" }
            0x9c { "Digital Signature" }
            0x9d { "Key Management" }
            0x9e { "Card Authentication" }
        }

        if ($slotsWithKeys -and ($slotsWithKeys | ForEach-Object { [byte]$_ }) -contains [byte]$slotByte) {
            if (-not $Force) {
                Write-Error "Slot '$Slot' already contains a private key that will be permanently destroyed. Use -Force to proceed."
                return
            }

        }

        # Report header
        Write-Host ""
        Write-Host "╔══════════════════════════════════════════════════════════════╗" -ForegroundColor Yellow
        Write-Host "║           YubiKey Key Generation Benchmark (PIV)             ║" -ForegroundColor Yellow
        Write-Host "╠══════════════════════════════════════════════════════════════╣" -ForegroundColor Yellow
        Write-Host "║$("  Device:       $($yubiKeyInfo.PrettyName)".PadRight(62))║" -ForegroundColor Yellow
        Write-Host "║$("  Serial:       $($yubiKeyInfo.SerialNumber)".PadRight(62))║" -ForegroundColor Yellow
        Write-Host "║$("  Firmware:     $($yubiKeyInfo.FirmwareVersion)".PadRight(62))║" -ForegroundColor Yellow
        Write-Host "║$("  Slot:         $slotName (0x$($slotByte.ToString('x2')))".PadRight(62))║" -ForegroundColor Yellow
        Write-Host "║$("  Iterations:   $Iterations".PadRight(62))║" -ForegroundColor Yellow
        Write-Host "║$("  Warmup:       $(if ($IncludeWarmup) { 'Yes' } else { 'No' })".PadRight(62))║" -ForegroundColor Yellow
        Write-Host "║$("  Algorithms:   $($algorithmsToTest -join ', ')".PadRight(62))║" -ForegroundColor Yellow
        Write-Host "║$("  Date:         $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')".PadRight(62))║" -ForegroundColor Yellow
        Write-Host "╚══════════════════════════════════════════════════════════════╝" -ForegroundColor Yellow

        # Benchmark loop
        $totalSteps = $algorithmsToTest.Count * $Iterations
        $currentStep = 0

        for ($algoIdx = 0; $algoIdx -lt $algorithmsToTest.Count; $algoIdx++) {
            $algo = $algorithmsToTest[$algoIdx]
            Write-Host "  Benchmarking $algo..." -ForegroundColor Cyan

            # Warmup
            if ($IncludeWarmup) {
                Write-Host "    Warmup iteration..." -ForegroundColor DarkGray
                try {
                    New-YubiKeyPIVKey -Slot $Slot -Algorithm $algo -PinPolicy Never -TouchPolicy Never -Confirm:$false | Out-Null
                    $keyInSlot = $true
                    if ($canDeleteKey) {
                        try {
                            Remove-YubiKeyPIVKey -Slot $Slot -Confirm:$false -WarningAction SilentlyContinue -ErrorAction Stop
                            $keyInSlot = $false
                        } catch {}
                    }
                }
                catch {
                    Write-Warning "    Warmup failed for ${algo}: $($_.Exception.Message)"
                }
            }

            # Timed iterations
            $iterationTimings = [System.Collections.Generic.List[double]]::new()
            $failCount = 0

            for ($i = 1; $i -le $Iterations; $i++) {
                $currentStep++
                $overallPercent = [math]::Round(($currentStep / $totalSteps) * 100)
                Write-Progress `
                    -Activity "Benchmarking:" `
                    -Status "$algo - Iteration $i of $Iterations" `
                    -PercentComplete $overallPercent `
                    -CurrentOperation "Algorithm $($algoIdx + 1) of $($algorithmsToTest.Count)"

                $sw = [System.Diagnostics.Stopwatch]::new()
                try {
                    $sw.Start()
                    New-YubiKeyPIVKey -Slot $Slot -Algorithm $algo -PinPolicy Never -TouchPolicy Never -Confirm:$false | Out-Null
                    $sw.Stop()
                    $keyInSlot = $true

                    $elapsedMs = $sw.Elapsed.TotalMilliseconds
                    $iterationTimings.Add($elapsedMs)

                    Write-Verbose "    Iteration ${i}: $([math]::Round($elapsedMs, 2)) ms"
                }
                catch {
                    $sw.Stop()
                    $failCount++
                    Write-Warning "    Iteration $i FAILED for ${algo}: $($_.Exception.Message)"
                }

                if ($canDeleteKey -and $keyInSlot) {
                    try {
                        Remove-YubiKeyPIVKey -Slot $Slot -Confirm:$false -WarningAction SilentlyContinue -ErrorAction Stop
                        $keyInSlot = $false
                    } catch {}
                }
            }

            # Per-algorithm statistics
            if ($iterationTimings.Count -gt 0) {
                $sorted = @($iterationTimings | Sort-Object)
                $count  = $sorted.Count
                $sum    = ($sorted | Measure-Object -Sum).Sum
                $avg    = $sum / $count
                $min    = $sorted[0]
                $max    = $sorted[$count - 1]

                $resultObj = [PSCustomObject]@{
                    Algorithm       = $algo
                    Iterations      = $count
                    Failed          = $failCount
                    FastestMs       = [math]::Round($min, 2)
                    SlowestMs       = [math]::Round($max, 2)
                    AverageMs       = [math]::Round($avg, 2)
                }
                $results.Add($resultObj)

                Write-Host "    $algo : avg $([math]::Round($avg, 1)) ms | fastest $([math]::Round($min, 1)) ms | slowest $([math]::Round($max, 1)) ms  ($count/$Iterations OK)" -ForegroundColor Green
            }
            else {
                $results.Add([PSCustomObject]@{
                    Algorithm       = $algo
                    Iterations      = 0
                    Failed          = $failCount
                    FastestMs       = $null
                    SlowestMs       = $null
                    AverageMs       = $null
                })

                Write-Host "    $algo : FAILED (all $Iterations iterations failed)" -ForegroundColor Red
            }
        }

        Write-Progress -Activity "Benchmarking:" -Completed

        # Cleanup: delete final key from slot (if supported)
        if ($canDeleteKey -and $keyInSlot) {
            try { Remove-YubiKeyPIVKey -Slot $Slot -Confirm:$false -WarningAction SilentlyContinue -ErrorAction SilentlyContinue } catch {}
            $keyInSlot = $false
        }

        # Output results
        Write-Output $results
    }
    finally {
        if ($canDeleteKey -and $keyInSlot) {
            try { Remove-YubiKeyPIVKey -Slot $Slot -Confirm:$false -WarningAction SilentlyContinue -ErrorAction SilentlyContinue } catch {}
        }
        if ($connected) {
            try { Disconnect-YubiKey -ErrorAction SilentlyContinue } catch {}
        }
    }
}

Measure-YubiKeyPIVKeyGeneration @args
