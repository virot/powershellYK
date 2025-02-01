Describe "FIDO2 Tests" -Tag @("FIDO2")  {
    It -Name "Verify connect to FIDO2" -Test {
        {Connect-YubikeyFIDO2 -PIN (ConvertTo-SecureString -String "612345" -AsPlainText -Force)} | Should -Throw
        {Connect-YubikeyFIDO2 -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)} | Should -Not -Throw
    }
    It -Name "Change PIN on FIDO2" -Test {
        {Set-YubikeyFIDO2PIN -OldPIN (ConvertTo-SecureString -String "612345" -AsPlainText -Force) -NewPIN (ConvertTo-SecureString -String "612345" -AsPlainText -Force)} | Should -Throw
        {Set-YubikeyFIDO2PIN -OldPIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force) -NewPIN (ConvertTo-SecureString -String "654321" -AsPlainText -Force)} | Should -Not -Throw
        {Connect-YubikeyFIDO2 -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)} | Should -Throw
        {Connect-YubikeyFIDO2 -PIN (ConvertTo-SecureString -String "654321" -AsPlainText -Force)} | Should -Not -Throw
        {Set-YubikeyFIDO2PIN -OldPIN (ConvertTo-SecureString -String "654321" -AsPlainText -Force) -NewPIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)} | Should -Not -Throw
    }
    It -Name "Clear all credentials" -Test {
        {Get-YubiKeyFIDO2Credential|%{Remove-YubikeyFIDO2Credential -CredentialId $_.CredentialID -Confirm:$false}} | Should -Not -Throw
	(Get-YubiKeyFIDO2Credential).Count | Should -Be 0
        {New-YubiKeyFIDO2Credential -RelyingPartyID 'powershellYK' -Challenge ([powershellYK.FIDO2.Challenge]::FakeChallange("powershellYK")) -Discoverable:$true -Username 'powershellYKUser' -UserID 0x01} | Should -Not -Throw
	(Get-YubiKeyFIDO2Credential).Count | Should -Be 1
	(Get-YubiKeyFIDO2Credential).UserName | Should -Be "powershellYKUser"
	(Get-YubiKeyFIDO2Credential).RPId | Should -Be "powershellYK"
	(Get-YubiKeyFIDO2Credential).Count | Should -Be 1
        {New-YubiKeyFIDO2Credential -RelyingPartyID 'powershellYK' -Challenge ([powershellYK.FIDO2.Challenge]::FakeChallange("powershellYK")) -Discoverable:$true -Username 'powershellYK' -UserID 0x02} | Should -Not -Throw
	(Get-YubiKeyFIDO2Credential).Count | Should -Be 2
        {Get-YubiKeyFIDO2Credential|%{Remove-YubikeyFIDO2Credential -CredentialId $_.CredentialID -Confirm:$false}} | Should -Not -Throw
	(Get-YubiKeyFIDO2Credential).Count | Should -Be 0
        {New-YubiKeyFIDO2Credential -RelyingPartyID 'powershellYK' -Challenge ([powershellYK.FIDO2.Challenge]::FakeChallange("powershellYK")) -Discoverable:$true -Username 'powershellYK' -UserID 0x03} | Should -Not -Throw
        $credentialString = (Get-YubiKeyFIDO2Credential|Select-Object -ExpandProperty CredentialID).ToString()
        {Remove-YubikeyFIDO2Credential -CredentialId $credentialString -Confirm:$false} | Should -Not -Throw
    }
}