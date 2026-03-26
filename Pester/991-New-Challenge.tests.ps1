Describe "New-Challenge" -Tag "Without-YubiKey" {

    BeforeEach {
        $outFile = [System.IO.Path]::GetTempFileName()
        Remove-Item $outFile -Force -ErrorAction SilentlyContinue
    }

    AfterEach {
        Remove-Item $outFile -Force -ErrorAction SilentlyContinue
    }

    It -Name "Creates file with default length (128 bytes)" -Test {
        New-Challenge -OutFile $outFile
        (Test-Path $outFile) | Should -Be $true
        (Get-Item $outFile).Length | Should -Be 128
    }

    It -Name "Defaults to challenge.bin in current directory when -OutFile omitted" -Test {
        $tempDir = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), [Guid]::NewGuid().ToString('n'))
        New-Item -ItemType Directory -Path $tempDir | Out-Null
        try {
            Push-Location $tempDir
            New-Challenge
            $defaultPath = Join-Path $tempDir 'challenge.bin'
            (Test-Path $defaultPath) | Should -Be $true
            (Get-Item $defaultPath).Length | Should -Be 128
        }
        finally {
            Pop-Location
            Remove-Item -LiteralPath $tempDir -Recurse -Force -ErrorAction SilentlyContinue
        }
    }

    It -Name "Creates file with -Length 256" -Test {
        New-Challenge -Length 256 -OutFile $outFile
        (Get-Item $outFile).Length | Should -Be 256
    }

    It -Name "Creates file with -Length 1 (min boundary)" -Test {
        New-Challenge -Length 1 -OutFile $outFile
        (Get-Item $outFile).Length | Should -Be 1
    }

    It -Name "Creates file with -Length 4096 (max boundary)" -Test {
        New-Challenge -Length 4096 -OutFile $outFile
        (Get-Item $outFile).Length | Should -Be 4096
    }

    It -Name "Generates different content on each call" -Test {
        Write-Verbose "$outFile"
        New-Challenge -Length 64 -OutFile $outFile
        $first = [System.IO.File]::ReadAllBytes($outFile)
        New-Challenge -Length 64 -OutFile $outFile -Force
        $second = [System.IO.File]::ReadAllBytes($outFile)
        ([System.BitConverter]::ToString($first) -ne [System.BitConverter]::ToString($second)) | Should -Be $true
    }

    It -Name "Overwrites existing file using -Force" -Test {
        [System.IO.File]::WriteAllBytes($outFile, [byte[]]@(1, 2, 3))
        New-Challenge -Length 16 -OutFile $outFile -Force
        (Get-Item $outFile).Length | Should -Be 16
    }

}

Describe "New-Challenge Errors" -Tag "Without-YubiKey" {
    BeforeAll {
        $outFile = [System.IO.Path]::GetTempFileName()
    }

    AfterAll {
        Remove-Item $outFile -Force -ErrorAction SilentlyContinue
    }

    It -Name "Length 0 throws (below range)" -Test {
        { New-Challenge -Length 0 -OutFile $outFile } | Should -Throw
    }

    It -Name "Length 4097 throws (above range)" -Test {
        { New-Challenge -Length 4097 -OutFile $outFile } | Should -Throw
    }

    It -Name "Overwrites existing file" -Test {
        { New-Challenge -Length 16 -OutFile $outFile } | Should -Throw
    }

}
