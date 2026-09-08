Describe "FIDO2 PRF Tests" -Tag @("FIDO2",'FIDO2prf')  {
    BeforeAll {
        Connect-YubiKey
        Connect-YubiKeyFIDO2 -PIN (ConvertTo-SecureString -String '123456' -AsPlainText -Force)
        New-YubiKeyFIDO2Credential -RelyingPartyID 'powershellYK-FIDO2-prf' -Challenge ([powershellYK.FIDO2.Challenge]::FakeChallange("powershellYK")) -Discoverable:$true -Username 'powershellYKUser' -UserID 0x01
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
        Protect-YubiKeyFIDO2File -Path $textin -Credential $cred -OutFile $encrypted -Confirm:$False
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

Describe "FIDO2 PRF AutoCreate Tests" -Tag @("FIDO2",'FIDO2prf')  {
    BeforeAll {
        Connect-YubiKey
        Connect-YubiKeyFIDO2 -PIN (ConvertTo-SecureString -String '123456' -AsPlainText -Force)
        Get-YubiKeyFIDO2Credential | Where-Object { $_.RPId -eq 'prf-encryption' } | ForEach-Object {
            Remove-YubikeyFIDO2Credential -CredentialId $_.CredentialID -Confirm:$false
        }
        $autoTextIn = [System.IO.Path]::GetTempFileName()
        Set-Content -Value "AutoCreate plaintext" -Path $autoTextIn
        $autoEncrypted = [System.IO.Path]::GetTempFileName()
        $autoEncryptedReuse = [System.IO.Path]::GetTempFileName()
        $autoTextOut = [System.IO.Path]::GetTempFileName()
        if (Test-Path $autoTextOut) { Remove-Item $autoTextOut }
        if (Test-Path $autoEncrypted) { Remove-Item $autoEncrypted }
        if (Test-Path $autoEncryptedReuse) { Remove-Item $autoEncryptedReuse }
    }
    AfterAll {
        Get-YubiKeyFIDO2Credential | Where-Object { $_.RPId -eq 'prf-encryption' } | ForEach-Object {
            Remove-YubikeyFIDO2Credential -CredentialId $_.CredentialID -Confirm:$false
        }
        if (Test-Path $autoTextIn) { Remove-Item $autoTextIn }
        if (Test-Path $autoEncrypted) { Remove-Item $autoEncrypted }
        if (Test-Path $autoEncryptedReuse) { Remove-Item $autoEncryptedReuse }
        if (Test-Path $autoTextOut) { Remove-Item $autoTextOut }
    }

    It -Name "AutoCreate encrypts without -Credential (hmac-secret-mc on 5.8+, two-step hmac-secret otherwise)" -Test {
        { Protect-YubiKeyFIDO2File -Path $autoTextIn -OutFile $autoEncrypted -Confirm:$False } | Should -Not -Throw
        (Test-Path $autoEncrypted) | Should -BeTrue
    }

    It -Name "Unprotect AutoCreate-encrypted file" -Test {
        { Unprotect-YubiKeyFIDO2File -Path $autoEncrypted -OutFile $autoTextOut -Confirm:$False } | Should -Not -Throw
        (Test-Path $autoTextOut) | Should -BeTrue
        (Get-Content -Path $autoTextOut) | Should -Be "AutoCreate plaintext"
    }

    It -Name "Second AutoCreate Protect reuses the prf-encryption credential" -Test {
        { Protect-YubiKeyFIDO2File -Path $autoTextIn -OutFile $autoEncryptedReuse -Confirm:$False } | Should -Not -Throw
        (Test-Path $autoEncryptedReuse) | Should -BeTrue
        $reuseOut = [System.IO.Path]::GetTempFileName()
        if (Test-Path $reuseOut) { Remove-Item $reuseOut }
        try {
            { Unprotect-YubiKeyFIDO2File -Path $autoEncryptedReuse -OutFile $reuseOut -Confirm:$False } | Should -Not -Throw
            (Get-Content -Path $reuseOut) | Should -Be "AutoCreate plaintext"
        }
        finally {
            if (Test-Path $reuseOut) { Remove-Item $reuseOut }
        }
    }
}