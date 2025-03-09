BeforeAll -Scriptblock {
}

Describe "New-YubikeyPIVCSR" {
    It -Name "Verify '-Slot 0x9a -Attestation' works" -Test {
	{New-YubikeyPIVKey -Slot 0x9c}
        {$attest = New-YubikeyPIVCSR -Slot 0x9c -Attestation -Subjectname 'CN=Pester,O=powershellYK,C=SE';
$verified = Confirm-YubiKeyAttestation -CertificateRequest $attest} | Should -Not -Throw
        $attest = New-YubikeyPIVCSR -Slot 0x9c -Attestation -Subjectname 'CN=Pester,O=powershellYK,C=SE';
        $attest | Should -BeOfType System.Byte
	$verified = Confirm-YubiKeyAttestation -CertificateRequest $attest
        $verified.AttestationMatchesCSR | Should -BeTrue
        $verified.AttestationValidated | Should -BeTrue
    }
}