Describe "Get-Challenge" -Tag "Without-YubiKey" {
    It -Name "Creates file with default length (128 bytes)" -Test {
        $outFile = Join-Path $env:TEMP "pester_challenge_$(Get-Random).bin"
        Get-Challenge -OutFile $outFile
        (Test-Path $outFile) | Should Be $true
        (Get-Item $outFile).Length | Should Be 128
        Remove-Item $outFile -Force -ErrorAction SilentlyContinue
    }

    It -Name "Creates file with -Length 256" -Test {
        $outFile = Join-Path $env:TEMP "pester_challenge_$(Get-Random).bin"
        Get-Challenge -Length 256 -OutFile $outFile
        (Get-Item $outFile).Length | Should Be 256
        Remove-Item $outFile -Force -ErrorAction SilentlyContinue
    }

    It -Name "Creates file with -Length 1 (min boundary)" -Test {
        $outFile = Join-Path $env:TEMP "pester_challenge_$(Get-Random).bin"
        Get-Challenge -Length 1 -OutFile $outFile
        (Get-Item $outFile).Length | Should Be 1
        Remove-Item $outFile -Force -ErrorAction SilentlyContinue
    }

    It -Name "Creates file with -Length 4096 (max boundary)" -Test {
        $outFile = Join-Path $env:TEMP "pester_challenge_$(Get-Random).bin"
        Get-Challenge -Length 4096 -OutFile $outFile
        (Get-Item $outFile).Length | Should Be 4096
        Remove-Item $outFile -Force -ErrorAction SilentlyContinue
    }

    It -Name "Generates different content on each call" -Test {
        $outFile = Join-Path $env:TEMP "pester_challenge_$(Get-Random).bin"
        Get-Challenge -Length 64 -OutFile $outFile
        $first = [System.IO.File]::ReadAllBytes($outFile)
        Get-Challenge -Length 64 -OutFile $outFile
        $second = [System.IO.File]::ReadAllBytes($outFile)
        ([System.BitConverter]::ToString($first) -ne [System.BitConverter]::ToString($second)) | Should Be $true
        Remove-Item $outFile -Force -ErrorAction SilentlyContinue
    }

    It -Name "Overwrites existing file" -Test {
        $overwritePath = Join-Path $env:TEMP "pester_overwrite_$(Get-Random).bin"
        [System.IO.File]::WriteAllBytes($overwritePath, [byte[]]@(1, 2, 3))
        Get-Challenge -Length 16 -OutFile $overwritePath
        (Get-Item $overwritePath).Length | Should Be 16
        Remove-Item $overwritePath -Force -ErrorAction SilentlyContinue
    }
}

Describe "Get-Challenge Errors" -Tag "Without-YubiKey" {
    It -Name "Length 0 throws (below range)" -Test {
        $threw = $false
        try { Get-Challenge -Length 0 -OutFile (Join-Path $env:TEMP "pester_challenge_err.bin") } catch { $threw = $true }
        $threw | Should Be $true
    }

    It -Name "Length 4097 throws (above range)" -Test {
        $threw = $false
        try { Get-Challenge -Length 4097 -OutFile (Join-Path $env:TEMP "pester_challenge_err.bin") } catch { $threw = $true }
        $threw | Should Be $true
    }
}
