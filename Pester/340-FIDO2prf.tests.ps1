Describe "FIDO2 Blob Tests" -Tag @("FIDO2",'FIDO2prf')  {
    BeforeAll {
        { Connect-YubiKey } | Should -Not -Throw
        { Connect-YubiKeyFIDO2 -PIN (ConvertTo-SecureString -String '123456' -AsPlainText -Force) } | Should -Not -Throw
        { New-YubiKeyFIDO2Credential -RelyingPartyID 'powershellYK-FIDO2-prf' -Challenge ([powershellYK.FIDO2.Challenge]::FakeChallange("powershellYK")) -Discoverable:$true -Username 'powershellYKUser' -UserID 0x01 } | Should -Not -Throw
        $textin = [System.IO.Path]::GetTempFileName()
        Set-Content -Value "Plaintext unencrypted file" -Path $textin
        $encrypted = [System.IO.Path]::GetTempFileName()
        $textout = [System.IO.Path]::GetTempFileName()
        if (Test-Path $textout) {Remove-Item $textout}
        if (Test-Path $encrypted) {Remove-Item $encrypted}
    }
    AfterAll {
        Remove-YubikeyFIDO2Credential -RelayingParty 'powershellYK-FIDO2-prf' -Username powershellYKUser -Confirm:$False
        if (Test-Path $textin) {Remove-Item $textin}
        if (Test-Path $encrypted) {Remove-Item $encrypted}
        if (Test-Path $textout) {Remove-Item $textout}
    }



    It -Name "Encrypt file with FIDO2 prf with specified outfile" -Test {
        $cred = Get-YubiKeyFIDO2Credential | Where-Object { $_.RelyingParty.Id -eq "powershellYK-FIDO2-prf" }
        { Protect-YubiKeyFIDO2File -Path $textin -Credential $cred -OutFile $encrypted -Confirm:$False} | Should -Not -Throw
        (Test-Path $encrypted) | Should -BeTrue
    }

    It -Name "Encrypt file with FIDO2 prf with out same as input+.enc" -Test {
        $cred = Get-YubiKeyFIDO2Credential | Where-Object { $_.RelyingParty.Id -eq "powershellYK-FIDO2-prf" }
        { Protect-YubiKeyFIDO2File -Path $textin -Credential $cred -Confirm:$False} | Should -Not -Throw
        (Test-Path "$($textin).enc") | Should -BeTrue
        Remove-Item "$($textin).enc"
    }

    It -Name "Decrypt file with FIDO2 prf" -Test {
        { Unprotect-YubiKeyFIDO2File -Path $encrypted -OutFile $textout -Confirm:$False} | Should -Not -Throw
        (Test-Path $textout) | Should -BeTrue
        (Get-Content -Path $textout) | Should -Be "Plaintext unencrypted file"
    }
}