BeforeAll -Scriptblock {
}

Describe "Assert-YubikeyPIV" {
    It -Name "Verify 'Assert-YubikeyPIV' works" -Test {
        {$attest = Assert-YubikeyPIV -Slot 0x9a} | Should -Not -Throw
        $attest | Should -BeOfType System.Security.Cryptography.X509Certificates.X509Certificate2
        $attest.Subject | Should -Be 'CN=YubiKey PIV Attestation 9a'
    }
}