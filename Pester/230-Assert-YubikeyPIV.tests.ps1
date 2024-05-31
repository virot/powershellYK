BeforeAll -Scriptblock {
}

Describe "Assert-YubikeyPIV" {
    It -Name "Verify 'Assert-YubikeyPIV' works" -Test {
        $attest = Assert-YubikeyPIV -Slot 0x9a
        $attest | Should -BeOfType System.Security.Cryptography.X509Certificates.X509Certificate2
        $attest.Subject | Should -Be 'CN=YubiKey PIV Attestation 9a'
    }
    It -Name "Invalid Slot" -Test {
        {Assert-YubikeyPIV -Slot 0x01} | Should -Throw
    }
}