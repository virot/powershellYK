BeforeAll -Scriptblock {
}

Describe "New-YubikeyPIVCSR" {
    It -Name "Verify '-Slot 0x9a -Attestation' works" -Test {
        {$attest = New-YubikeyPIVCSR -Slot 0x9a -Attestation -Subjectname 'CN=Pester,O=VirotYubikey,C=SE'} | Should -Not -Throw
        $attest | Should -BeOfType System.Byte[]
        {$verified = $verified = Confirm-YubikeyAttestion -CertificateRequest $a} | Should -Not -Throw
        $verified.AttestionMatchesCSR | Should -BeTrue
        $verified.AttestionValidated | Should -BeTrue
    }
}