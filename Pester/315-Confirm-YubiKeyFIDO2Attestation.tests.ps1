Describe "Confirm-YubiKeyFIDO2Attestation paths" -Tag "Without-YubiKey" {
    It -Name "Verify 'Confirm-YubiKeyFIDO2Attestation -AttestationObject _fullfile_' works, Windows" -Skip:(!$IsWindows) -Test {
        $pest_return = Confirm-YubiKeyFIDO2Attestation -AttestationObject "$PSScriptRoot\TestData\attestation.bin"
        $pest_return.GetType().FullName | Should -Be 'powershellYK.FIDO2.FIDO2AttestationResult'
        $pest_return.AttestationValidated | Should -Be $true
        $pest_return.AttestationPath | Should -Not -Be $null
        ($pest_return.AttestationPath.Count -ge 1) | Should -Be $true
    }

    It -Name "Verify 'Confirm-YubiKeyFIDO2Attestation -AttestationObject _fullfile_' works, Non-Windows" -Skip:($IsWindows) -Test {
        $pest_return = Confirm-YubiKeyFIDO2Attestation -AttestationObject "$PSScriptRoot/TestData/attestation.bin"
        $pest_return.GetType().FullName | Should -Be 'powershellYK.FIDO2.FIDO2AttestationResult'
        $pest_return.AttestationValidated | Should -Be $true
        $pest_return.AttestationPath | Should -Not -Be $null
    }

    It -Name "Verify 'Confirm-YubiKeyFIDO2Attestation -AttestationObject _shortfile_' works, Windows" -Skip:(!$IsWindows) -Test {
        $pest_return = Confirm-YubiKeyFIDO2Attestation -AttestationObject ".\Pester\TestData\attestation.bin"
        $pest_return.GetType().FullName | Should -Be 'powershellYK.FIDO2.FIDO2AttestationResult'
        $pest_return.AttestationValidated | Should -Be $true
    }

    It -Name "Verify 'Confirm-YubiKeyFIDO2Attestation -AttestationObject _shortfile_' works, Non-Windows" -Skip:($IsWindows) -Test {
        $pest_return = Confirm-YubiKeyFIDO2Attestation -AttestationObject "./Pester/TestData/attestation.bin"
        $pest_return.GetType().FullName | Should -Be 'powershellYK.FIDO2.FIDO2AttestationResult'
        $pest_return.AttestationValidated | Should -Be $true
    }
}

Describe "Confirm-YubiKeyFIDO2Attestation output" -Tag 'Without-YubiKey' {
    It -Name "Verify AttestationPath contains Yubico root" -Test {
        $pest_return = Confirm-YubiKeyFIDO2Attestation -AttestationObject "$PSScriptRoot\TestData\attestation.bin"
        ($pest_return.AttestationPath -join ' ') | Should -Match 'Yubico'
    }
}

Describe "Confirm-YubiKeyFIDO2Attestation Errors" -Tag 'Without-YubiKey' {
    It -Name "Missing file throws" -Test {
        $badPath = Join-Path $PSScriptRoot "TestData\nonexistent_attestation_315_test.bin"
        (Test-Path $badPath) | Should -Be $false
        $threw = $false
        try { Confirm-YubiKeyFIDO2Attestation -AttestationObject $badPath } catch { $threw = $true }
        $threw | Should -Be $true
    }

    It -Name "Invalid format throws" -Test {
        $threw = $false
        try { Confirm-YubiKeyFIDO2Attestation -AttestationObject "$PSScriptRoot\TestData\rsa_2048_cert.pem" } catch { $threw = $true }
        $threw | Should -Be $true
    }
}
