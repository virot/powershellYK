BeforeAll -Scriptblock {
}

Describe "Assert-YubikeyPIV" -Tag "PIV" {
    It -Name "Verify 'Assert-YubikeyPIV' works" -Test {
        {Reset-YubikeyPIV -Confirm:$false} | Should -Not -Throw
        {Connect-YubikeyPIV -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)} | Should -Not -Throw
	{New-YubikeyPIVKey -Slot "0x9c" -PinPolicy Default -Algorithm EcP256 -TouchPolicy Never} | Should -Not -Throw
	{New-YubikeyPIVSelfSign -Slot "0x9c"} | Should -Not -Throw
        $attest = Assert-YubikeyPIV -Slot 0x9c
        $attest | Should -BeOfType System.Security.Cryptography.X509Certificates.X509Certificate2
        $attest.Subject | Should -Be 'CN=YubiKey PIV Attestation 9c'
    }

    It -Name "Invalid Slot" -Test {
        {Assert-YubikeyPIV -Slot 0x01} | Should -Throw
    }
}