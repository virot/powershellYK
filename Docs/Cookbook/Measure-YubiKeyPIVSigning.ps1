<#
.SYNOPSIS
Benchmarks PIV signing performance across supported algorithms on a YubiKey.

.DESCRIPTION
Measures signing throughput by generating a key per algorithm, then calling
PivSession.Sign() directly against the YubiKey secure element. This is a raw
signing benchmark with no certificate or CSR overhead.

For ECC keys, the input is a random digest sized to the curve (32 bytes for P-256,
48 bytes for P-384). For RSA keys, the input is a PKCS#1 v1.5 padded SHA-256 digest
sized to the key modulus.

Curve25519 algorithms (Ed25519, X25519) are automatically excluded as the PIV
signing interface does not support them.

Results are returned as PSCustomObject arrays suitable for piping to Format-Table
or ConvertTo-Json.

.PARAMETER Iterations
Number of timed signing iterations per algorithm. Defaults to 5.

.PARAMETER Algorithm
Optional filter to benchmark only specific algorithms. If omitted, all signable
algorithms supported by the connected YubiKey are benchmarked (Curve25519 excluded).

.PARAMETER Slot
PIV slot to use for key generation and signing. Defaults to "Card Authentication" (0x9e).

.PARAMETER IncludeWarmup
Runs one untimed signing iteration per algorithm before the timed iterations to
eliminate PIV applet initialization overhead.

.PARAMETER Force
Allows the benchmark to proceed when the target slot already contains a private key.
Without this switch, the script aborts to prevent accidental key destruction.

.EXAMPLE
.\Measure-YubiKeyPIVSigning.ps1
Benchmarks signing with all supported algorithms, 5 iterations each.

.EXAMPLE
.\Measure-YubiKeyPIVSigning.ps1 -Iterations 20 -Algorithm Rsa2048
Benchmarks RSA 2048 signing with 20 iterations.

.EXAMPLE
.\Measure-YubiKeyPIVSigning.ps1 -IncludeWarmup
Benchmarks all algorithms with a warmup pass.

.EXAMPLE
.\Measure-YubiKeyPIVSigning.ps1 -Slot "Digital Signature" -Force
Benchmarks signing in the Digital Signature slot, overwriting any existing key.

.EXAMPLE
.\Measure-YubiKeyPIVSigning.ps1 -Slot 0x9c -Force
Same as above, using the hex slot identifier instead of the friendly name.

.NOTES
- Requires the powershellYK module
- Uses slot 0x9e (Card Authentication) with PIN and touch policy disabled
- Aborts if the target slot already contains a key unless -Force is specified
- Curve25519 (Ed25519/X25519) is excluded as it does not support PIV signing
- Keys are generated once per algorithm as setup, then signed repeatedly
- Signing is performed via PivSession.Sign() directly (no CSR or certificate overhead)
- Key cleanup after benchmarking requires firmware 5.7+; older firmware leaves the last key in the slot
- Timings include USB/host overhead and vary with OS, USB connection type, drivers, host load etc.
- Results reflect end-to-end signing latency, not raw secure element performance.

.LINK
https://github.com/virot/powershellYK/
#>

#Requires -PSEdition Core
#Requires -Modules powershellYK

function Measure-YubiKeyPIVSigning {
    [CmdletBinding()]
    param(
        [Parameter(HelpMessage = "Number of timed signing iterations per algorithm")]
        [ValidateRange(1, 100)]
        [int]$Iterations = 5,

        [Parameter(HelpMessage = "Algorithm(s) to benchmark; defaults to all signable")]
        [ValidateSet("Rsa1024", "Rsa2048", "Rsa3072", "Rsa4096", "EcP256", "EcP384")]
        [string[]]$Algorithm,

        [Parameter(HelpMessage = "PIV slot to use for key generation and signing")]
        [ArgumentCompletions("PIV Authentication", "Digital Signature", "Key Management", "Card Authentication", "0x9a", "0x9c", "0x9d", "0x9e")]
        [string]$Slot = "Card Authentication",

        [Parameter(HelpMessage = "Run one untimed warmup signing iteration per algorithm")]
        [switch]$IncludeWarmup,

        [Parameter(HelpMessage = "Overwrite any existing key in the target slot without prompting")]
        [switch]$Force
    )

    # A helper to build correctly formatted input for PivSession.Sign()
    function New-SignInput {
        param([string]$AlgorithmName)

        $sha256DigestInfo = [byte[]]@(
            0x30, 0x31, 0x30, 0x0d, 0x06, 0x09, 0x60, 0x86,
            0x48, 0x01, 0x65, 0x03, 0x04, 0x02, 0x01, 0x05,
            0x00, 0x04, 0x20
        )

        switch -Wildcard ($AlgorithmName) {
            "EcP256" {
                $digest = [byte[]]::new(32)
                [System.Security.Cryptography.RandomNumberGenerator]::Fill($digest)
                return ,$digest
            }
            "EcP384" {
                $digest = [byte[]]::new(48)
                [System.Security.Cryptography.RandomNumberGenerator]::Fill($digest)
                return ,$digest
            }
            "Rsa*" {
                $keySizeBytes = switch ($AlgorithmName) {
                    "Rsa1024" { 128 }
                    "Rsa2048" { 256 }
                    "Rsa3072" { 384 }
                    "Rsa4096" { 512 }
                }

                $hash = [byte[]]::new(32)
                [System.Security.Cryptography.RandomNumberGenerator]::Fill($hash)

                $digestInfoBlock = [byte[]]::new($sha256DigestInfo.Length + $hash.Length)
                [Array]::Copy($sha256DigestInfo, 0, $digestInfoBlock, 0, $sha256DigestInfo.Length)
                [Array]::Copy($hash, 0, $digestInfoBlock, $sha256DigestInfo.Length, $hash.Length)

                $padded = [byte[]]::new($keySizeBytes)
                $padded[0] = 0x00
                $padded[1] = 0x01
                $paddingLength = $keySizeBytes - 3 - $digestInfoBlock.Length
                for ($p = 2; $p -lt (2 + $paddingLength); $p++) { $padded[$p] = 0xFF }
                $padded[2 + $paddingLength] = 0x00
                [Array]::Copy($digestInfoBlock, 0, $padded, (3 + $paddingLength), $digestInfoBlock.Length)

                return ,$padded
            }
            default {
                throw "Unsupported algorithm for signing: $AlgorithmName"
            }
        }
    }

    $curveAlgorithms = @("Ed25519", "X25519")

    $results    = [System.Collections.Generic.List[PSCustomObject]]::new()
    $canDeleteKey = $false
    $connected  = $false
    $keyInSlot  = $false
    $pivSession = $null

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
            $curve = $Algorithm | Where-Object { $_ -in $curveAlgorithms }
            if ($curve) {
                Write-Warning "Curve25519 algorithms do not support signing and will be skipped: $($curve -join ', ')"
            }
            $unsupported = $Algorithm | Where-Object { $_ -notin $supportedAlgorithms -and $_ -notin $curveAlgorithms }
            if ($unsupported) {
                Write-Warning "The following algorithms are not supported by this YubiKey and will be skipped: $($unsupported -join ', ')"
            }
            $algorithmsToTest = @($Algorithm | Where-Object { $_ -in $supportedAlgorithms -and $_ -notin $curveAlgorithms })
        }
        else {
            $algorithmsToTest = @($supportedAlgorithms | Where-Object { $_ -notin $curveAlgorithms })
        }

        if (-not $algorithmsToTest -or $algorithmsToTest.Count -eq 0) {
            Write-Error "No signable algorithms to benchmark."
            return
        }

        # Determine if created key(s) can be deleted following test
        $canDeleteKey = $yubiKeyInfo.FirmwareVersion -ge [Yubico.YubiKey.FirmwareVersion]::new(5, 7, 0)
        if (-not $canDeleteKey) {
            Write-Warning "Firmware does not support key deletion (a test key will remain in the slot)."
        }

        # Detect and warn the user if there is an existing key in the target slot
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

        $slotsWithKeys = $pivInfo.SlotsWithPrivateKeys
        if ($slotsWithKeys -and ($slotsWithKeys | ForEach-Object { [byte]$_ }) -contains [byte]$slotByte) {
            if (-not $Force) {
                Write-Error "Slot '$Slot' already contains a private key that will be permanently destroyed. Use -Force to proceed."
                return
            }

        }

        # Report header
        Write-Host ""
        Write-Host "╔══════════════════════════════════════════════════════════════╗" -ForegroundColor Yellow
        Write-Host "║               YubiKey Signing Benchmark (PIV)                ║" -ForegroundColor Yellow
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

            # Setup: generate key for this algorithm (untimed)
            Write-Host "  [$algo] Generating key..." -ForegroundColor DarkGray
            try {
                New-YubiKeyPIVKey -Slot $Slot -Algorithm $algo -PinPolicy Never -TouchPolicy Never -Confirm:$false | Out-Null
                $keyInSlot = $true
            }
            catch {
                Write-Warning "  Failed to generate $algo key: $($_.Exception.Message)"
                $results.Add([PSCustomObject]@{
                    Algorithm       = $algo
                    Iterations      = 0
                    Failed          = $Iterations
                    FastestMs       = $null
                    SlowestMs       = $null
                    AverageMs       = $null
                    SigningsPerSec  = $null
                })
                $currentStep += $Iterations
                continue
            }

            # Prepare sign input and open PIV session
            $signInput = New-SignInput -AlgorithmName $algo
            try {
                $pivSession = [Yubico.YubiKey.Piv.PivSession]::new([powershellYK.YubiKeyModule]::_yubikey)
                $pivSession.KeyCollector = [powershellYK.YubiKeyModule]::_KeyCollector.YKKeyCollectorDelegate
            }
            catch {
                Write-Warning "  Failed to open PIV session for ${algo}: $($_.Exception.Message)"
                $results.Add([PSCustomObject]@{
                    Algorithm       = $algo
                    Iterations      = 0
                    Failed          = $Iterations
                    FastestMs       = $null
                    SlowestMs       = $null
                    AverageMs       = $null
                    SigningsPerSec  = $null
                })
                $currentStep += $Iterations
                continue
            }

            Write-Host "  [$algo] Benchmarking signing..." -ForegroundColor Cyan

            # Warmup
            if ($IncludeWarmup) {
                Write-Host "    Warmup iteration..." -ForegroundColor DarkGray
                try {
                    $pivSession.Sign([byte]$slotByte, [System.ReadOnlyMemory[byte]]$signInput) | Out-Null
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
                    -Status "$algo signing - Iteration $i of $Iterations" `
                    -PercentComplete $overallPercent `
                    -CurrentOperation "Algorithm $($algoIdx + 1) of $($algorithmsToTest.Count)"

                $sw = [System.Diagnostics.Stopwatch]::new()
                try {
                    $sw.Start()
                    $pivSession.Sign([byte]$slotByte, [System.ReadOnlyMemory[byte]]$signInput) | Out-Null
                    $sw.Stop()

                    $elapsedMs = $sw.Elapsed.TotalMilliseconds
                    $iterationTimings.Add($elapsedMs)

                    Write-Verbose "    Iteration ${i}: $([math]::Round($elapsedMs, 2)) ms"
                }
                catch {
                    $sw.Stop()
                    $failCount++
                    Write-Warning "    Iteration $i FAILED for ${algo}: $($_.Exception.Message)"
                }
            }

            # Dispose PIV session
            try { $pivSession.Dispose() } catch {}
            $pivSession = $null

            #Per-algorithm statistics
            if ($iterationTimings.Count -gt 0) {
                $sorted = @($iterationTimings | Sort-Object)
                $count  = $sorted.Count
                $sum    = ($sorted | Measure-Object -Sum).Sum
                $avg    = $sum / $count
                $min    = $sorted[0]
                $max    = $sorted[$count - 1]
                $sigPerSec = if ($avg -gt 0) { [math]::Round(1000 / $avg, 1) } else { 0 }

                $resultObj = [PSCustomObject]@{
                    Algorithm       = $algo
                    Iterations      = $count
                    Failed          = $failCount
                    FastestMs       = [math]::Round($min, 2)
                    SlowestMs       = [math]::Round($max, 2)
                    AverageMs       = [math]::Round($avg, 2)
                    SigningsPerSec  = $sigPerSec
                }
                $results.Add($resultObj)

                Write-Host "    $algo : avg $([math]::Round($avg, 1)) ms | $sigPerSec signings/sec  ($count/$Iterations OK)" -ForegroundColor Green
            }
            else {
                $results.Add([PSCustomObject]@{
                    Algorithm       = $algo
                    Iterations      = 0
                    Failed          = $failCount
                    FastestMs       = $null
                    SlowestMs       = $null
                    AverageMs       = $null
                    SigningsPerSec  = $null
                })

                Write-Host "    $algo : FAILED (all $Iterations iterations failed)" -ForegroundColor Red
            }

            # Teardown: delete key before next algorithm
            if ($canDeleteKey) {
                try { Remove-YubiKeyPIVKey -Slot $Slot -Confirm:$false -WarningAction SilentlyContinue -ErrorAction SilentlyContinue } catch {}
                $keyInSlot = $false
            }
        }

        Write-Progress -Activity "Benchmarking:" -Completed

        # Final cleanup
        if ($canDeleteKey -and $keyInSlot) {
            try { Remove-YubiKeyPIVKey -Slot $Slot -Confirm:$false -WarningAction SilentlyContinue -ErrorAction SilentlyContinue } catch {}
            $keyInSlot = $false
        }

        # Output results
        Write-Output $results
    }
    finally {
        if ($null -ne $pivSession) {
            try { $pivSession.Dispose() } catch {}
        }
        if ($canDeleteKey -and $keyInSlot) {
            try { Remove-YubiKeyPIVKey -Slot $Slot -Confirm:$false -WarningAction SilentlyContinue -ErrorAction SilentlyContinue } catch {}
        }
        if ($connected) {
            try { Disconnect-YubiKey -ErrorAction SilentlyContinue } catch {}
        }
    }
}

Measure-YubiKeyPIVSigning @args
