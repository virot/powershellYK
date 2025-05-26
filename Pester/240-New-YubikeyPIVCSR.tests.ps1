BeforeAll -Scriptblock {
}

Describe "New-YubikeyPIVCSR" -Tag "PIV" {
    It -Name "Verify '-Slot 0x9a -Attestation' works" -Test {
        $tempfile = [System.IO.Path]::GetTempFileName()
        Remove-Item $tempfile
	{New-YubikeyPIVKey -Slot 0x9c}
        {$attest = New-YubikeyPIVCSR -Slot 0x9c -Attestation -Subjectname 'CN=Pester,O=powershellYK,C=SE' -OutFile $tempfile;
        $verified = Confirm-YubiKeyAttestation -CertificateRequestFile $tempfile} | Should -Not -Throw
        Remove-Item $tempfile
        $attest = New-YubikeyPIVCSR -Slot 0x9c -Attestation -Subjectname 'CN=Pester,O=powershellYK,C=SE' -OutFile $tempfile;
	$verified = Confirm-YubiKeyAttestation -CertificateRequestFile $tempfile
        $verified.AttestationMatchesCSR | Should -BeTrue
        $verified.AttestationValidated | Should -BeTrue
    }
}